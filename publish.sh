echo "scp太慢干掉的时候要先删除"
rm -rf ~/RiderProjects/Asura/Asura/Publish/
echo "ssh 到服务器上停止服务！"
ssh jd "systemctl stop asura-dev;exit"
echo "进入项目目录准备编译发布"
cd Asura/
dotnet publish -c Release -o Publish --runtime ubuntu.16.04-x64
echo "复制项目文件到远程服务器上"
scp -r ~/RiderProjects/Asura/Asura/Publish jd:/var/www/asura
echo "ssh 到服务器上重启服务！"
ssh jd "systemctl restart asura-dev;exit"
echo "删除发布文件夹，否则下次调试会失败"
rm -rf ~/RiderProjects/Asura/Asura/Publish/