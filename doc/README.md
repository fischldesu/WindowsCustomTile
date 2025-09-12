## 使用说明 wiki

使用说明文档 [请点击这里](https://repo.fischldesu.com/WindowsCustomTile/wiki)
English [Basic Wiki](https://github.com/fischldesu/WindowsCustomTile/wiki)   

## 开发计划 路线

- v0.1 release (20250817)
- [ ] UI逻辑优化
- [x] 更优的启动命令逻辑
[6d23db3](https://github.com/fischldesu/WindowsCustomTile/commit/367a34eb2759e6985a94567da030bc744a7e77f4)

- v0.1.1
- [ ] 优化软件体积(代码裁剪 直接默认裁剪会导致`WCTCore`中`IBackgroundTask`无法触发)
- v0.1.x

- [ ] 半可视化磁贴编辑器
- v0.2

- [ ] 通过HTTP请求(可能类Py爬虫)获取流媒体、博客等消息展示到磁贴 [#3](https://github.com/fischldesu/WindowsCustomTile/issues/3)

- v0.3

## 已知问题

1. 当前登录账户为`Administrator`将会始终打开窗口  
而对于本身需要管理员运行的`EXE`等文件，会导致并不能直接启动/通过UAC使用户获取权限  
当前算是设计缺陷 后续会改进
暂时解决办法：使用bat cmd 等脚本中间层执行
2. 变化的动态磁贴（幻灯片、磁贴队列等）在一些情况下会失效（不显示、停止）
3. 部分动态磁贴内容在更新时无法生效，始终是上一次的内容（如幻灯片磁贴）  
解决办法：来回的切换磁贴大小

## 开发日志
See [Git Commit](https://github.com/fischldesu/WindowsCustomTile/commits/master/)
