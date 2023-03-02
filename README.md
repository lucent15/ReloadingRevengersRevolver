# ReloadingRevengersRevolver
[ゲームはこちら](https://unityroom.com/games/reloadingrevengersrevolver)

ゲームジャムのお題「Re」から、リロードを連想し、西部劇でお馴染みの銃のリロードを細かく体験できるゲームを作ろうと思って開発しました。同時にドット絵も興味が湧いていたので、ドット絵も自分で描いています。transformの回転でリボルバーを表現しようとするとドット絵が崩れてしまうので、2コマで表現した回転するシリンダーの上に、穴の位置に合わせて弾の画像を連動させています。プログラムではint型の配列でシリンダーの状態「空」「弾入り」「撃った後」の三つの状態を表して画像と連動させ、プレイヤーが任意に操作したリボルバーの状態がドット絵のアニメーションでも違和感なく表現できたと思います。リロードが複雑な分他はシンプルな画面クリックによる射撃ゲームにしてバランスを取り、敵を撃つ、敵の攻撃から身を隠す（対処する）というシューティングの根本的楽しさを併せることで銃撃戦ゲームの基本的な核を楽しめるゲームにできたと思っています。


以下ソースコードの大まかな内容です  


SoundManagerは[こちらの記事](https://i-school.memo.wiki/d/SoundManager%A4%C7%A5%B2%A1%BC%A5%E0%C6%E2%A4%CE%B2%BB%B8%BB%A4%F2%B4%C9%CD%FD%A4%B9%A4%EB)からコードをお借りしました。


DamageCaller.cs：敵の部位ごとのダメージを取得する

EnemyController.cs：敵のHPや動きの制御

HowToPlay.cs：あそびかた/射撃場での操作説明のテキストを表示するため

PlayerController.cs：プレイヤーやリボルバーの操作全般やアニメーションなど

RevolverAnimationOffset.cs：リボルバーのAnimation Eventで関数を呼ぶための仲介用

SceneController.cs：シーンの遷移を制御

TotalGameDirector.cs：スコアの記録やゲームの進行管理を行う
