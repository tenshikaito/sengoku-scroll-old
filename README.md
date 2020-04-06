# sengoku-scroll
A game like Nobunaga's Ambition(KOEI)、 Sangokushi(KOEI) 、Aoki Ookami to Shiroki Mejika - Genghis Khan(KOEI)、Taikou Risshiden(KOEI)、Civilization(Firaxis)、Europa Universalis(Paradox)

Q: 为啥你的代码还是.net fw，core不能用么？
A: 这代码是从core上改回.net的
Q: 震惊，为啥
A: 暴几个原因、一个是本来就只能在windows上跑所以也不指望跨平台、另一个原来打算有一部分UI内容是用内嵌浏览器去写部分图标绘制界面
而core基本上没有支持的库、也就是不成熟
而且以目前的架构什么时候改回core去也没多大工作量、都是winform

Q: 居然不是WPF，不过貌似也用不到XAML
A: wpf不熟、网上的资料也太少了、学习成本太高太花时间、改过wpf但是原来的winform也留着
如果有人愿意帮我把UI抄去wpf也没问题

Q: 做游戏还不如sharp DX？
A: sharpdx这个库用了之后觉得太底层、学起来太花时间、好多东西也没封装到能直接用、资料也太少

Q: 毕竟就是个图形库
可这样的话为啥没直接用unity？
A: 游戏需要的UI很复杂、而且需要一些底层绘制接口、同时也不喜欢图形编辑界面、最重要的是想用c井.NET最新版本
总之现阶段先把游戏系统搞完能跑、接口都封装好了以后想迁移图形库还是框架什么的都方便


