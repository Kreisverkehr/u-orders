[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![Release][release-shield]][release-url]
[![Docker][docker-shield]][docker-url]
[![MIT License][license-shield]][license-url]

# µOrders
Or micro-orders or u-orders is a small web app that provides self service ordering capabilities.

> :warning: **Not sutable for production**: This is not intended for any real world use. This project can't be used for any accounting or financial requirements your location may have.

## Ok, I got the warning. How do I get started?
This app is designed to run in several, distributed containers. Although it can be run as seperate processes on one maschine I will only describe the way it is designed to be run. The app is composed of 4 components:
- WebUI - Serves the web app
- Main API - Backend for the webapp
- Printer API - handles printing of the orders
- Background Services - maintains the Database and clears out old Data

You can deploy these together with docker compose. Here is an example:
```yaml
version: '3.4'

services:

  db:
    image: mysql:oracle
    restart: always
    volumes:
      - db:/var/lib/mysql:rw
    environment:
      - MYSQL_ROOT_PASSWORD=rootpwd
      - MYSQL_USER=dev
      - MYSQL_PASSWORD=devpwd
      - MYSQL_DATABASE=devdb
    networks:
      - backend
  
  printer:
    image: ghcr.io/kreisverkehr/u-orders:print-latest
    restart: always
    environment:
      - Printer__Type=File
      - Printer__CodePage=858
      - Printer__FilePath=/dev/usb/lp0
      - Printer__Culture=de-DE
      - AuthSecret=ThisIsThePrinterSecret
    devices:
      - "/dev/usb/lp0:/dev/usb/lp0"
    networks:
      - backend

  service:
    image: ghcr.io/kreisverkehr/u-orders:service-latest
    restart: always
    environment:
      - Db__Provider=mysql
      - Db__Host=db
      - Db__Port=3306
      - Db__User=dev
      - Db__Password=devpwd
      - Db__DbName=devdb
      - ADMIN_USER=admin
      - ADMIN_PASS=4u-Orders
    depends_on:
      - db
      - printer
    networks:
      - backend

  api:
    image: ghcr.io/kreisverkehr/u-orders:api-latest
    restart: always
    depends_on:
      - db
      - service
    environment:
      - Db__Provider=mysql
      - Db__Host=db
      - Db__Port=3306
      - Db__User=dev
      - Db__Password=devpwd
      - Db__DbName=devdb
      - JWT__Secret=ThisIsTheWebApiSecret
      - Printer__Host=printer
      - Printer__Port=80
      - Printer__Lang=de
      - Printer__Secret=ThisIsThePrinterSecret
      - PRINT_SERVICE_HOST=printer
      - PRINT_SERVICE_PORT=80
    networks:
      - frontend
      - backend

  webui:
    image: ghcr.io/kreisverkehr/u-orders:web-latest
    restart: always
    volumes:
      - ./appsettings.json:/usr/share/nginx/html/appsettings.json
    depends_on:
      - api
    networks:
      - frontend

networks:
  backend:
  frontend:

volumes:
  db:
```

You'll also need an additional file called `appsettings.json`. There you can provide the settings for the web app.
```json
{
  "api": {
    "host": "my-api-server.example.com",
    "port": "443",
    "scheme": "https"
  },
  "Brand": {
    "Name": "µOrders",
    "Description": "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam",
    "Icon": "Store",
    "Currency": "EUR"
  }
}
```

If done right a simpe `docker compose up` schould get the app up and running. Depending on your maschine it may take a while to set everything up.

## I'm into it, but can you provide feature x?
First of all: This is a hobby project. Feel free to open up a feature request or even better, implement it yourself and open up a PR on main. I'll look into it as soon as I can.

<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/Kreisverkehr/u-orders.svg?logo=github&style=for-the-badge
[contributors-url]: https://github.com/Kreisverkehr/u-orders/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Kreisverkehr/u-orders.svg?logo=github&style=for-the-badge
[forks-url]: https://github.com/Kreisverkehr/u-orders/network/members
[stars-shield]: https://img.shields.io/github/stars/Kreisverkehr/u-orders.svg?logo=github&style=for-the-badge
[stars-url]: https://github.com/Kreisverkehr/u-orders/stargazers
[issues-shield]: https://img.shields.io/github/issues/Kreisverkehr/u-orders.svg?logo=github&style=for-the-badge
[issues-url]: https://github.com/Kreisverkehr/u-orders/issues
[license-shield]: https://img.shields.io/github/license/Kreisverkehr/u-orders.svg?style=for-the-badge
[license-url]: https://github.com/Kreisverkehr/u-orders/blob/main/LICENSE
[release-shield]: https://img.shields.io/github/downloads/Kreisverkehr/u-orders/total?logo=github&style=for-the-badge
[release-url]: https://github.com/Kreisverkehr/u-orders/releases/latest
[docker-shield]: https://img.shields.io/docker/pulls/kreisverkehr/u-orders?logo=docker&style=for-the-badge
[docker-url]: https://hub.docker.com/r/kreisverkehr/discord-event-bot
