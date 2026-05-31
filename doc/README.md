## 使用说明 wiki

使用说明文档 [请点击这里](https://repo.fischldesu.com/WindowsCustomTile/wiki)
English [Basic Wiki](https://github.com/fischldesu/WindowsCustomTile/wiki)   

## 开发计划 路线

- v0.1 release (20250817)
- [x] UI逻辑优化
[6d23db3](https://github.com/fischldesu/WindowsCustomTile/commit/6d23db36a69a920fcb5cc404a1046db33a22f52d)
[40d6786](https://github.com/fischldesu/WindowsCustomTile/commit/40d67866b0b859ebf58da02cda6d415ada9a2234)
- [x] 更优的启动命令逻辑
[367a34e](https://github.com/fischldesu/WindowsCustomTile/commit/367a34eb2759e6985a94567da030bc744a7e77f4)
- v0.1.1 release (20260531)
- [x] 启动提权([#7](https://github.com/fischldesu/WindowsCustomTile/issues/7) [#8](https://github.com/fischldesu/WindowsCustomTile/issues/8)) 
[314d1db](https://github.com/fischldesu/WindowsCustomTile/commit/314d1db29291d9de00d87d726eeaa32aafccb41c)
[5de733b](https://github.com/fischldesu/WindowsCustomTile/commit/5de733b73737c265689a9d0b74a5a16edf67ab94)
- v0.1.2
- [ ] 优化软件体积(代码裁剪 当前情况下直接默认裁剪会导致`WCTCore`中`IBackgroundTask`无法触发)
- v0.1.x  
...
- v0.2
- [ ] 半可视化磁贴编辑器

- [ ] 通过HTTP请求(可能类Py爬虫)获取流媒体、博客等消息展示到磁贴 [#3](https://github.com/fischldesu/WindowsCustomTile/issues/3)

- v0.3

## 已知问题

0. See [issue](https://github.com/fischldesu/WindowsCustomTile/issues)
1. 变化的动态磁贴（幻灯片、磁贴队列等）在一些情况下会失效（不显示、停止）
2. 部分动态磁贴内容在更新时无法生效，始终是上一次的内容（如幻灯片磁贴）  
解决办法：来回的切换磁贴大小

## 开发日志
See [Git Commit](https://github.com/fischldesu/WindowsCustomTile/commits/master/)
