using AutoMapper;
using UOrders.DTOModel.V1;
using UOrders.DTOModel.V1.Authentication;
using UOrders.EFModel;

namespace UOrders.Api.Extensions;

public static class IMapperConfigurationExpressionExtensions
{
    public static void ConfigureApiV1(this IMapperConfigurationExpression cfg)
    {
        string lang = string.Empty;

        cfg.ReplaceMemberName("Prices", "CurrentPrice");
        cfg.ReplaceMemberName("Price", "Prices");

#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
        cfg.CreateMap<IEnumerable<LocalizedText>, string>()
            .ForCtorParam("value", opts => opts.MapFrom(src => src.OrderByDescending(t => t.Lang).FirstOrDefault(t => t.Lang == lang || t.Lang == "").Text.ToCharArray()))
            ;
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.

        cfg.CreateMap<KeyValuePair<string, string>, LocalizedText>()
            .ConstructUsing(src => new LocalizedText(src.Key, src.Value))
            .ForAllMembers(opts => opts.Ignore())
            ;

        cfg.CreateMap<LocalizedText, KeyValuePair<string, string>>()
            .ForCtorParam("key", opts => opts.MapFrom(src => src.Lang))
            .ForCtorParam("value", opts => opts.MapFrom(src => src.Text))
            .ForAllMembers(opts => opts.Ignore())
            ;

        cfg.CreateMap<IEnumerable<LocalizedText>, IDictionary<string, string>>()
            .ConstructUsing(src => new Dictionary<string, string>())
            .AfterMap((src, dest) =>
            {
                foreach (var item in src)
                    dest.Add(item.Lang, item.Text);
            })
            ;

        cfg.CreateMap<IEnumerable<Price>, decimal?>()
            .ForCtorParam("value", opts => opts.MapFrom(src => src.First(p => p.ValidFrom < DateTimeOffset.Now && (!p.ValidTo.HasValue || p.ValidTo.Value >= DateTimeOffset.Now)).Value))
            ;

        cfg.CreateMap<decimal?, IEnumerable<Price>>()
            .ConstructUsing(src => new List<Price>() { new Price() { ValidFrom = DateTimeOffset.Now, Value = src ?? 0 } })
            .ForAllMembers(opts => opts.Ignore())
            ;

        cfg.CreateMap<MenuCategory, MenuCategoryDTO>()
            .ForMember(dest => dest.Items, opts => opts.MapFrom(src => src.Items.Where(i => !i.ToBeRemoved)))
            ;

        cfg.CreateMap<MenuCategory, MenuCategoryDetailedDTO>()
            .ForMember(dest => dest.Items, opts => opts.MapFrom(src => src.Items.Where(i => !i.ToBeRemoved)))
            ;

        cfg.CreateMap<MenuCategoryCreateDTO, MenuCategory>()
            .AfterMap((src, dest) =>
            {
                dest.Title.AddDefaultLanguageIfNeeded();
                dest.Description.AddDefaultLanguageIfNeeded();
            })
            ;

        cfg.CreateMap<MenuCategoryUpdateDTO, MenuCategory>()
            .AfterMap((src, dest) =>
            {
                dest.Title.AddDefaultLanguageIfNeeded();
                dest.Description.AddDefaultLanguageIfNeeded();
            })
            ;

        cfg.CreateMap<MenuItem, MenuItemDTO>()
            .ForMember(dest => dest.Options, opts => opts.MapFrom(src => src.Options.Where(o => !o.ToBeRemoved)))
            ;

        cfg.CreateMap<MenuItemOption, MenuItemOptionDTO>()
            .ForMember(dest => dest.Values, opts => opts.MapFrom(src => src.Values.Where(v => !v.ToBeRemoved)))
            ;

        cfg.CreateMap<MenuItemOptionValue, MenuItemOptionValueDTO>()
            ;

        cfg.CreateMap<MenuItemOption, MenuItemOptionDetailedDTO>()
            .ForMember(dest => dest.Values, opts => opts.MapFrom(src => src.Values.Where(v => !v.ToBeRemoved)))
            ;

        cfg.CreateMap<MenuItemOptionCreateDTO, MenuItemOption>()
            .AfterMap((src, dest) =>
            {
                dest.Name.AddDefaultLanguageIfNeeded();
                dest.Description.AddDefaultLanguageIfNeeded();
            })
            ;

        cfg.CreateMap<MenuItemOptionUpdateDTO, MenuItemOption>()
            .AfterMap((src, dest) =>
            {
                dest.Name.AddDefaultLanguageIfNeeded();
                dest.Description.AddDefaultLanguageIfNeeded();
            })
            ;

        cfg.CreateMap<MenuItemOptionValue, MenuItemOptionValueDetailedDTO>()
            ;

        cfg.CreateMap<MenuItemOptionValueCreateDTO, MenuItemOptionValue>()
            .AfterMap((src, dest) =>
            {
                dest.Name.AddDefaultLanguageIfNeeded();
            })
            ;

        cfg.CreateMap<MenuItemOptionValueUpdateDTO, MenuItemOptionValue>()
            .AfterMap((src, dest) =>
            {
                dest.Name.AddDefaultLanguageIfNeeded();
            })
            ;

        cfg.CreateMap<MenuItem, MenuItemDetailedDTO>()
            .ForMember(dest => dest.Options, opts => opts.MapFrom(src => src.Options.Where(o => !o.ToBeRemoved)))
            ;

        cfg.CreateMap<MenuItemCreateDTO, MenuItem>()
            .ForMember(dest => dest.Prices, opts => opts.NullSubstitute(0m))
            .AfterMap((src, dest) =>
            {
                dest.Title.AddDefaultLanguageIfNeeded();
                dest.Description.AddDefaultLanguageIfNeeded();
            })
            ;

        cfg.CreateMap<MenuItemUpdateDTO, MenuItem>()
            .AfterMap((src, dest) =>
            {
                dest.Title.AddDefaultLanguageIfNeeded();
                dest.Description.AddDefaultLanguageIfNeeded();
            })
            ;

        cfg.CreateMap<OrderCreateDTO, Order>()
            ;

        cfg.CreateMap<OrderCreateItemDTO, OrderItem>()
            .ForMember(dest => dest.CheckedOptions, opts => opts.Ignore())
            ;

        cfg.CreateMap<Order, OrderDTO>()
            ;

        cfg.CreateMap<Order, OrderOverviewDTO>()
            ;

        cfg.CreateMap<IEnumerable<OrderItem>, int?>()
            .ForCtorParam("value", opts => opts.MapFrom(src => src.Count()))
            ;

        cfg.CreateMap<OrderItem, OrderItemDTO>()
            ;

        cfg.CreateMap<OrderItemCheckedOption, OrderItemOptionDTO>()
            ;

        cfg.CreateMap<UOrdersUser, UserDTO>()
            .ForMember(dest => dest.Phone, opts => opts.MapFrom(src => src.PhoneNumber))
            ;

        cfg.CreateMap<OrderReview, OrderReviewDTO>()
            ;

        cfg.CreateMap<OrderReview, ReviewDTO>()
            .ForMember(dest => dest.OrderedOn, opts => opts.MapFrom(src => src.Order.OrderedOn))
            .ForMember(dest => dest.Name, opts => opts.AddTransform(src => src != null ? src.ShortenName() : src));
            ;

        cfg.CreateMap<OrderReviewCreateDTO, OrderReview>()
            ;
    }
}
