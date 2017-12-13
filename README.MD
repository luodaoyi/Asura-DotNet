# Asura

这是个.net core mvc 和entityframeworkcore 用 sqlite数据库写的单人博客

DEMO: [https://dev.luodaoyi.com](https://dev.luodaoyi.com)
>正在慢慢完成中。

感谢[https://imququ.com/](https://imququ.com/)和[https://deepzz.com/](https://deepzz.com/) 两位大佬

他们的博客几乎是瞬间打开

所以我也想用 .net core实现一遍。以便于熟悉和巩固asp.net core的和entityframeworkcore相关的知识，并且拥有一个速度非常非常快的博客系统

## 开发调试

```
dotnet watch run
```

## 发布部署

```
dotnet publish -c Release -o Publish --runtime ubuntu.16.04-x64 
```

此方式发布后目标操作系统无需安装运行时

支持的系统 

### windows

- 可移植
  - `win-x86`
  - `win-x64`
- Windows 7 / Windows Server 2008 R2
  - `win7-x64`
  - `win7-x86`
- Windows 8 / Windows Server 2012
  - `win8-x64`
  - `win8-x86`
  - `win8-arm`
- Windows 8.1 / Windows Server 2012 R2
  - `win81-x64`
  - `win81-x86`
  - `win81-arm`
- Windows 10 / Windows Server 2016
  - `win10-x64`
  - `win10-x86`
  - `win10-arm`
  - `win10-arm64`

### Linux

- 可移植
  - `linux-x64`
- CentOS
  - `centos-x64`
  - `centos.7-x64`
- Debian
  - `debian-x64`
  - `debian.8-x64`
- Fedora
  - `fedora-x64`
  - `fedora.24-x64`
  - `fedora.25-x64`（.NET Core 2.0 或更高版本）
  - `fedora.26-x64`（.NET Core 2.0 或更高版本）
- Gentoo（.NET Core 2.0 或更高版本）
  - `gentoo-x64`
- openSUSE
  - `opensuse-x64`
  - `opensuse.42.1-x64`
- Oracle Linux
  - `ol-x64`
  - `ol.7-x64`
  - `ol.7.0-x64`
  - `ol.7.1-x64`
  - `ol.7.2-x64`
- Red Hat Enterprise Linux
  - `rhel-x64`
  - `rhel.6-x64`（.NET Core 2.0 或更高版本）
  - `rhel.7-x64`
  - `rhel.7.1-x64`
  - `rhel.7.2-x64`
  - `rhel.7.3-x64`（.NET Core 2.0 或更高版本）
  - `rhel.7.4-x64`（.NET Core 2.0 或更高版本）
- Tizen（.NET Core 2.0 或更高版本）
  - `tizen`
- Ubuntu
  - `ubuntu-x64`
  - `ubuntu.14.04-x64`
  - `ubuntu.14.10-x64`
  - `ubuntu.15.04-x64`
  - `ubuntu.15.10-x64`
  - `ubuntu.16.04-x64`
  - `ubuntu.16.10-x64`
- Ubuntu 衍生内容
  - `linuxmint.17-x64`
  - `linuxmint.17.1-x64`
  - `linuxmint.17.2-x64`
  - `linuxmint.17.3-x64`
  - `linuxmint.18-x64`
  - `linuxmint.18.1-x64`（.NET Core 2.0 或更高版本）

## macos

- `osx-x64`(.NET 核心 2.0 或更高版本，最低版本是`osx.10.12-x64`)
- `osx.10.10-x64`
- `osx.10.11-x64`
- `osx.10.12-x64`（.NET Core 1.1 或更高版本）
- `osx.10.13-x64`

## 部署.net服务

上一步发布的包复制到服务器的目录上
以下配置请自己替换第一步发布的包锁拷贝的地址

```
sudo vim /etc/systemd/system/asura-dev.service 

输入以下内容：


[Unit]
Description=我的小博客开发版本

[Service]
WorkingDirectory=/var/www/asura/Publish
ExecStart=/var/www/asura/Publish/Asura
Restart=always
RestartSec=10
SyslogIdentifier=dotnet-example
User=root
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target


然后启用服务

sudo systemctl enable asura-dev.service

启动：
sudo systemctl start asura-dev.service
查看状态：
sudo systemctl status asura-dev.service
查看日志
sudo journalctl -fu asura-dev.service
```

## 部署反向代理

```
sudo wget -N --no-check-certificate https://raw.githubusercontent.com/luodaoyi/SimplesScript/master/caddy_install.sh 
sudo chmod +x caddy_install.sh 
sudo ./caddy_install.sh install http.authz,http.cache,http.cgi,http.cors,http.expires,http.filemanager,http.filter,http.forwardproxy,http.ipfilter,http.minify,http.nobots,http.proxyprotocol,http.ratelimit,http.realip

完毕后生成反向代理配置:

echo "https://yourdomain.com {
 root /var/www/your-path
 gzip
 minify
 tls your-email
 proxy / http://127.0.0.1:5000 {
  except /js /images /css
 }
}" > /usr/local/caddy/Caddyfile


然后重启caddy

sudo service caddy restart
```