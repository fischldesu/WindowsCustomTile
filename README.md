# Windows Custom Tile

Makes you quickly customize Windows start menu tiles.  
个性化你的Windows开始菜单磁贴

![Application home-page Screenshot](/Assets/screenshot-main.png)

[开发博客](https://blog.fischldesu.com/?p=windows-live-tile) /
[Bilibili](https://www.bilibili.com/video/BV1T83ozJETg) /
[下载](https://github.com/fischldesu/WindowsCustomTile/release)  
[网站 WindowsCustomTile.fischldesu.com](https://repo.fischldesu.com/WindowsCustomTile)

### Tech Stack 技术栈
`.NET` with `WindowsAppSDK` `WinUI 3` `WinRT`

磁贴更新 `Windows.UI.Notifications.TileUpdater`
```
TileUpdater tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
Windows.Data.Xml.Dom.XmlDocument tileXml = new();

tileXml.LoadXml(<Here goes your tile Xml string>);
TileNotification tileNotification = new(tileXml);
tileUpdater.Update(tileNotification);
```

### Installation 直接安装
1. 包中含有一个证书文件`WindowsCustomTile_xxx_x64.cer`，安装这个证书文件
2. 安装证书时，注意安装到本地计算机，选择`将所有的证书都放入下列存储`
3. 位置选`受信任的根证书颁发机构`
4. 证书安装完成后，打开`WindowsCustomTile_xxx_x64.msix`即可安装应用程序

### Instructions 使用说明
启动APP后

`Launch Command` 启动命令，在输入栏中输入需要启动的命令，比如想要启动一个软件，就输入软件exe位置，点击Apply，启动App后会自动执行你的指令，若没确定之前有勾选`Always show window`，则不会显示窗口。

`Image Tile` 快速设置图片磁贴点击右侧`Image file`按钮，选择图片文件(jpg、png等等)后，左侧出现预览图，点击`Submit`即可应用图片磁贴，下方可以单独编辑某个尺寸的磁贴图链接(或直接选择图片文件)，点击`Apply`后再点击`Submit`即可应用更改。

⚠️ 由于`Windows11`取消了开始菜单磁贴，若要使用磁贴  
推荐使用三方软件“调出Win10遗留的”开始菜单：  
[Github@valinet/ExplorerPatcher](https://github.com/valinet/ExplorerPatcher)

<br>  
This software codes are licensed under the MIT License.
