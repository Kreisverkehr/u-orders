using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Sockets;
using UOrders.DTOModel.V1.Authentication;
using UOrders.EFModel;
using UOrders.EFModel.Options;

namespace UOrders.Api.Extensions;

public static class IHostExtensions
{
    #region Public Methods

    public static async Task CreateAdminIfNeeded<HostType>(this HostType host) where HostType : IHost
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<UOrdersUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var config = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<HostType>>();

        if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        if (!await roleManager.RoleExistsAsync(UserRoles.User))
            await roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        logger.LogInformation("No admin users present. Creating admin-user as specified.");

        UOrdersUser? user = null;
        if ((user = await userManager.FindByNameAsync(config["ADMIN_USER"])) == null)
        {
            user = new()
            {
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = config["ADMIN_USER"]
            };

            var result = await userManager.CreateAsync(user, config["ADMIN_PASS"]);
            if (!result.Succeeded)
            {
                var reason = string.Join("\n", result.Errors.Select(err => $"{err.Code}: {err.Description}"));
                logger.LogError("Could not create admin user. Reason: {reason}", reason);
                return;
            }
            await userManager.AddToRoleAsync(user, UserRoles.User);
        }

        if (await roleManager.RoleExistsAsync(UserRoles.Admin) && !await userManager.IsInRoleAsync(user, UserRoles.Admin))
        {
            await userManager.AddToRoleAsync(user, UserRoles.Admin);
        }
    }

    public static void MigrateDb<ContextType>(this IHost host) where ContextType : DbContext
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ContextType>();
        context.Database.Migrate();
    }

    public static void SeedData(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var _dbContext = services.GetRequiredService<UOrdersDbContext>();

        if (!_dbContext.Categories.Any())
        {
            var defaultPizzaOptions = new List<MenuItemOption>()
            {
                new MenuItemOption()
                {
                    Name = new List<LocalizedText>()
                    {
                        new LocalizedText()
                        {
                            Lang = "de",
                            Text = "Größe"
                        },
                        new LocalizedText()
                        {
                            Lang = "en",
                            Text = "Size"
                        },
                        new LocalizedText()
                        {
                            Lang = "",
                            Text = "Size"
                        }
                    },
                    Description = new List<LocalizedText>()
                    {
                        new LocalizedText()
                        {
                            Lang = "de",
                            Text = "Größe der Pizza"
                        },
                        new LocalizedText()
                        {
                            Lang = "en",
                            Text = "Size of your pizza"
                        },
                        new LocalizedText()
                        {
                            Lang = "",
                            Text = "Size of your pizza"
                        }
                    },
                    OptionType = MenuItemOptionType.Selection,
                    Values = new List<MenuItemOptionValue>()
                    {
                        new MenuItemOptionValue()
                        {
                            Name = new List<LocalizedText>()
                            {
                                new LocalizedText()
                                {
                                    Lang="de",
                                    Text="Klein"
                                },
                                new LocalizedText()
                                {
                                    Lang="en",
                                    Text="small"
                                },
                                new LocalizedText()
                                {
                                    Lang="",
                                    Text="xs"
                                }
                            },
                            PriceChangeToBase = new List<Price>()
                            {
                                new Price()
                                {
                                    ValidFrom = DateTimeOffset.Now,
                                    Value = 0
                                }
                            }
                        },
                        new MenuItemOptionValue()
                        {
                            Name = new List<LocalizedText>()
                            {
                                new LocalizedText()
                                {
                                    Lang="de",
                                    Text="Mittel"
                                },
                                new LocalizedText()
                                {
                                    Lang="en",
                                    Text="medium"
                                },
                                new LocalizedText()
                                {
                                    Lang="",
                                    Text="m"
                                }
                            },
                            PriceChangeToBase = new List<Price>()
                            {
                                new Price()
                                {
                                    ValidFrom = DateTimeOffset.Now,
                                    Value = 1
                                }
                            }
                        },
                        new MenuItemOptionValue()
                        {
                            Name = new List<LocalizedText>()
                            {
                                new LocalizedText()
                                {
                                    Lang="de",
                                    Text="Groß"
                                },
                                new LocalizedText()
                                {
                                    Lang="en",
                                    Text="large"
                                },
                                new LocalizedText()
                                {
                                    Lang="",
                                    Text="xl"
                                }
                            },
                            PriceChangeToBase = new List<Price>()
                            {
                                new Price()
                                {
                                    ValidFrom = DateTimeOffset.Now,
                                    Value = 2
                                }
                            }
                        }
                    }
                },
                new MenuItemOption()
                {
                    Name = new List<LocalizedText>()
                    {
                        new LocalizedText()
                        {
                            Lang = "de",
                            Text = "Extras"
                        },
                        new LocalizedText()
                        {
                            Lang = "en",
                            Text = "Extras"
                        },
                        new LocalizedText()
                        {
                            Lang = "",
                            Text = "Extras"
                        }
                    },
                    Description = new List<LocalizedText>()
                    {
                        new LocalizedText()
                        {
                            Lang = "de",
                            Text = "Gewünschte Extra Toppings"
                        },
                        new LocalizedText()
                        {
                            Lang = "en",
                            Text = "Desired extra toppings"
                        },
                        new LocalizedText()
                        {
                            Lang = "",
                            Text = "Desired extra toppings"
                        }
                    },
                    OptionType = MenuItemOptionType.MultiSelect,
                    Values = new List<MenuItemOptionValue>()
                    {
                        new MenuItemOptionValue()
                        {
                            Name = new List<LocalizedText>()
                            {
                                new LocalizedText()
                                {
                                    Lang="de",
                                    Text="Mozzerella"
                                },
                                new LocalizedText()
                                {
                                    Lang="en",
                                    Text="Mozzerella"
                                },
                                new LocalizedText()
                                {
                                    Lang="",
                                    Text="Mozzerella"
                                }
                            },
                            PriceChangeToBase = new List<Price>()
                            {
                                new Price()
                                {
                                    ValidFrom = DateTimeOffset.Now,
                                    Value = .3m
                                }
                            }
                        },
                        new MenuItemOptionValue()
                        {
                            Name = new List<LocalizedText>()
                            {
                                new LocalizedText()
                                {
                                    Lang="de",
                                    Text="Salami"
                                },
                                new LocalizedText()
                                {
                                    Lang="en",
                                    Text="Salami"
                                },
                                new LocalizedText()
                                {
                                    Lang="",
                                    Text="Salami"
                                }
                            },
                            PriceChangeToBase = new List<Price>()
                            {
                                new Price()
                                {
                                    ValidFrom = DateTimeOffset.Now,
                                    Value = .3m
                                }
                            }
                        },
                        new MenuItemOptionValue()
                        {
                            Name = new List<LocalizedText>()
                            {
                                new LocalizedText()
                                {
                                    Lang="de",
                                    Text="Thunfisch"
                                },
                                new LocalizedText()
                                {
                                    Lang="en",
                                    Text="Tuna"
                                },
                                new LocalizedText()
                                {
                                    Lang="",
                                    Text="Tuna"
                                }
                            },
                            PriceChangeToBase = new List<Price>()
                            {
                                new Price()
                                {
                                    ValidFrom = DateTimeOffset.Now,
                                    Value = .4m
                                }
                            }
                        },
                        new MenuItemOptionValue()
                        {
                            Name = new List<LocalizedText>()
                            {
                                new LocalizedText()
                                {
                                    Lang="de",
                                    Text="Brokkoli"
                                },
                                new LocalizedText()
                                {
                                    Lang="en",
                                    Text="Broccoli"
                                },
                                new LocalizedText()
                                {
                                    Lang="",
                                    Text="Broccoli"
                                }
                            },
                            PriceChangeToBase = new List<Price>()
                            {
                                new Price()
                                {
                                    ValidFrom = DateTimeOffset.Now,
                                    Value = .4m
                                }
                            }
                        }
                    }
                }
            };

            _dbContext.Categories.Add(new MenuCategory()
            {
                Title = new List<LocalizedText>()
                {
                    new LocalizedText()
                    {
                        Lang = "de",
                        Text ="Pizza"
                    },
                    new LocalizedText()
                    {
                        Lang = "en",
                        Text ="Pizza"
                    },
                    new LocalizedText()
                    {
                        Lang = "",
                        Text ="Pizza"
                    }
                },
                Description = new List<LocalizedText>()
                {
                    new LocalizedText()
                    {
                        Lang = "de",
                        Text = "Leckere Pizza"
                    },
                    new LocalizedText()
                    {
                        Lang = "en",
                        Text = "Delicious pizza"
                    },
                    new LocalizedText()
                    {
                        Lang = "",
                        Text = "Delicious pizza"
                    }
                },
                Items = new List<MenuItem>()
                {
                    new MenuItem()
                    {
                        Title = new List<LocalizedText>()
                        {
                            new LocalizedText()
                            {
                                Lang = "de",
                                Text ="Pizza Salami"
                            },
                            new LocalizedText()
                            {
                                Lang = "en",
                                Text ="Pizza Salami"
                            },
                            new LocalizedText()
                            {
                                Lang = "",
                                Text ="Pizza Salami"
                            }
                        },
                        Description= new List<LocalizedText>()
                        {
                            new LocalizedText()
                            {
                                Lang = "de",
                                Text ="Tomatensauce, Käse, Salami"
                            },
                            new LocalizedText()
                            {
                                Lang = "en",
                                Text ="Tomato sauce, cheese, salami"
                            },
                            new LocalizedText()
                            {
                                Lang = "",
                                Text ="Tomato sauce, cheese, salami"
                            }
                        },
                        Prices = new List<Price>()
                        {
                            new Price()
                            {
                                Value = 2.5m,
                                ValidFrom = DateTimeOffset.Now
                            }
                        },
                        Options = defaultPizzaOptions.Select(o => (MenuItemOption)o.Clone()).ToList()
                    },
                    new MenuItem()
                    {
                        Title = new List<LocalizedText>()
                        {
                            new LocalizedText()
                            {
                                Lang = "de",
                                Text ="Pizza Thunfisch"
                            },
                            new LocalizedText()
                            {
                                Lang = "en",
                                Text ="Pizza tuna"
                            },
                            new LocalizedText()
                            {
                                Lang = "",
                                Text ="Pizza tuna"
                            }
                        },
                        Description= new List<LocalizedText>()
                        {
                            new LocalizedText()
                            {
                                Lang = "de",
                                Text ="Tomatensauce, Käse, Thunfisch"
                            },
                            new LocalizedText()
                            {
                                Lang = "en",
                                Text ="Tomato sauce, cheese, tuna"
                            },
                            new LocalizedText()
                            {
                                Lang = "",
                                Text ="Tomato sauce, cheese, tuna"
                            }
                        },
                        Prices = new List<Price>()
                        {
                            new Price()
                            {
                                Value = 2.6m,
                                ValidFrom = DateTimeOffset.Now
                            }
                        },
                        Options = defaultPizzaOptions.Select(o => (MenuItemOption)o.Clone()).ToList()
                    },
                    new MenuItem()
                    {
                        Title = new List<LocalizedText>()
                        {
                            new LocalizedText()
                            {
                                Lang = "de",
                                Text ="Pizza Brokkoli"
                            },
                            new LocalizedText()
                            {
                                Lang = "en",
                                Text ="Pizza broccoli"
                            },
                            new LocalizedText()
                            {
                                Lang = "",
                                Text ="Pizza broccoli"
                            }
                        },
                        Description= new List<LocalizedText>()
                        {
                            new LocalizedText()
                            {
                                Lang = "de",
                                Text ="Tomatensauce, Käse, Brokkoli"
                            },
                            new LocalizedText()
                            {
                                Lang = "en",
                                Text ="Tomato sauce, cheese, broccoli"
                            },
                            new LocalizedText()
                            {
                                Lang = "",
                                Text ="Tomato sauce, cheese, broccoli"
                            }
                        },
                        Prices = new List<Price>()
                        {
                            new Price()
                            {
                                Value = 2.4m,
                                ValidFrom = DateTimeOffset.Now
                            }
                        },
                        Options = defaultPizzaOptions.Select(o => (MenuItemOption)o.Clone()).ToList()
                    }
                }
            });
            _dbContext.Categories.Add(new MenuCategory()
            {
                Title = new List<LocalizedText>()
                {
                    new LocalizedText()
                    {
                        Lang = "de",
                        Text ="Pasta"
                    },
                    new LocalizedText()
                    {
                        Lang = "en",
                        Text ="Pasta"
                    },
                    new LocalizedText()
                    {
                        Lang = "",
                        Text ="Pasta"
                    }
                },
                Description = new List<LocalizedText>()
                {
                    new LocalizedText()
                    {
                        Lang = "de",
                        Text = "Leckere Pasta"
                    },
                    new LocalizedText()
                    {
                        Lang = "en",
                        Text = "Delicious Pasta"
                    },
                    new LocalizedText()
                    {
                        Lang = "",
                        Text = "Delicious Pasta"
                    }
                },
                Items = new List<MenuItem>()
                {
                    new MenuItem()
                    {
                        Title = new List<LocalizedText>()
                        {
                            new LocalizedText()
                            {
                                Lang = "de",
                                Text ="Spagetthi Bolognese"
                            },
                            new LocalizedText()
                            {
                                Lang = "en",
                                Text ="Spagetthi Bolognese"
                            },
                            new LocalizedText()
                            {
                                Lang = "",
                                Text ="Spagetthi Bolognese"
                            }
                        },
                        Description= new List<LocalizedText>()
                        {
                            new LocalizedText()
                            {
                                Lang = "de",
                                Text ="Spagetthi, Hackfleischsoße"
                            },
                            new LocalizedText()
                            {
                                Lang = "en",
                                Text ="Spagetthi, minced meat sauce"
                            },
                            new LocalizedText()
                            {
                                Lang = "",
                                Text ="Spagetthi, minced meat sauce"
                            }
                        },
                        Prices = new List<Price>()
                        {
                            new Price()
                            {
                                Value = 2.5m,
                                ValidFrom = DateTimeOffset.Now
                            }
                        }
                    },
                    new MenuItem()
                    {
                        Title = new List<LocalizedText>()
                        {
                            new LocalizedText()
                            {
                                Lang = "de",
                                Text ="Spagetthi Carbonara"
                            },
                            new LocalizedText()
                            {
                                Lang = "en",
                                Text ="Spagetthi carbonara"
                            },
                            new LocalizedText()
                            {
                                Lang = "",
                                Text ="Spagetthi carbonara"
                            }
                        },
                        Description= new List<LocalizedText>()
                        {
                            new LocalizedText()
                            {
                                Lang = "de",
                                Text ="Spagetthi, Schinken-Sahne-Soße"
                            },
                            new LocalizedText()
                            {
                                Lang = "en",
                                Text ="Spagetthi, ham and cream sauce"
                            },
                            new LocalizedText()
                            {
                                Lang = "",
                                Text ="Spagetthi, ham and cream sauce"
                            }
                        },
                        Prices = new List<Price>()
                        {
                            new Price()
                            {
                                Value = 2.6m,
                                ValidFrom = DateTimeOffset.Now
                            }
                        }
                    }
                }
            });
            _dbContext.SaveChanges();
        }
    }

    public static void WaitForDb<HostType>(this HostType host, TimeSpan? waitTime = null, int retries = 0, Action? dbUnreachableCallback = null) where HostType : IHost
    {
        if (waitTime == null) waitTime = TimeSpan.Zero;

        var dbConfig = host.Services.GetRequiredService<IOptions<Db>>().Value;

        using TcpClient tcpClient = new();
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<HostType>>();
        var config = services.GetRequiredService<IConfiguration>();

        logger.LogInformation("Waiting for DB to come up.");

        int tries = 0;
        bool checkDb = true;
        while (checkDb)
        {
            try
            {
                tcpClient.Connect(dbConfig.Host, dbConfig.Port);
                checkDb = false;
            }
            catch (Exception)
            {
                logger.LogWarning("Db at {dbHost}:{dbPort} not up yet", dbConfig.Host, dbConfig.Port);
                if (retries > 0 && ++tries >= retries)
                {
                    logger.LogError("Db unreachable after {tries} tries.", tries);
                    dbUnreachableCallback?.Invoke();
                }
                Thread.Sleep(waitTime.Value);
            }
        }
    }

    #endregion Public Methods
}