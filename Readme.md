# TJAPlayerPI
最終更新日:2025/11/16(Takkkom)

このReadmeはTJAPlayer3-fのReadmeを基に作成いたしました。

## はじめに
このソフトウェアは、オープンソースな太鼓シミュレーターです。  
このソフトウェアは、TakkkomがTJAPlayer3-fという太鼓譜面ビューアーを改造したものです。  

* 太鼓さん次郎・TJAPlayer等で使われている.tjaファイル
* Koioto等に使われている.tcc .tcm .tciファイル

を読み込み、再生することができます。  
(すべての対応ファイルが読み込めるわけではありません。)   


## 使用上の注意
* TJAPlayerPIはオープンソースソフトウェアです。このソフトウェア・スキンはすべてMITライセンスに準拠します。
* プログラムの制作者(Takkkom)は、TJAPlayerPI本体(GitHubからのダウンロード)とデフォルトのスキンのサポートのみ行います。
* すべての環境で動作確認はできないので、動いたら運がいい、程度でお願いします。
* 常時60fpsを保てないPCでの動作は期待できません。
* このプログラムを使用し発生した、いかなる不具合・損失に対しても、一切の責任を負いません。  
  このソフトウェアを使用する場合は、**全て自己責任**でお願いします。


## 動画、配信等でのご利用について
TJAPlayerPIを動画共有サイトやライブ配信サービス、ウェブサイトやブログ等でご利用になられる場合、  
既存の商用ゲームソフトウェアのものでないこと、他のソフトウェアと混同しないよう配慮をお願いいたします。  


## TJAPlayerPIの改造・再配布(二次配布)を行う場合について
TJAPlayerPI、デフォルトスキンはMITライセンスで制作されています。  
MITライセンスのルールのもと、改造・再配布を行うことは自由ですが、**全て自己責任**でお願いします。  
また、使用しているライブラリのライセンス上、**必ず**「ThirdPartyLicenses」フォルダを同梱の上、改造・再配布をお願いします。  
外部スキンや、譜面パッケージを同梱する場合は、それぞれの制作者のルールや規約を守ってください。  
これらにTJAPlayerPIのライセンスは適用されません。


## 界隈に対してのちょっとキツメな非難と開発者からのお願い
太鼓シミュは結構やばい歴史を辿ってきました。悪意のあるユーザーによって騒ぎが起きて開発が終了したりすることもざらです。
特に二次配布によっての配布終了は数えきれないほどあります。
MITライセンスは自由です自由。改造するも良し、壊すもよし、学ぶも良し、一部を使うもよし。
でもねえ.....だからと言ってなんでも許させるわけではないです。その許されないことはあなたの想像通りかもしれないし、そうじゃないかもしれないです。

- TJAPlayerPIの開発者や貢献者は「ルールの偽善」を超非難します。例えばTJAPlayerPIのスキンを作って配布したときに第三者がそのスキンを改造したりほかの用途に使っても文句を言わないでください。要するに.....大事なことなので三回言います。「あなたが作ったTJAPlayerPIのスキンが二次配布されても文句言わないでください!!!!」「あなたが作ったTJAPlayerPIのスキンが二次配布されても文句言わないでください!!!!」「あなたが作ったTJAPlayerPIのスキンが二次配布されても文句言わないでください!!!!」そもそも太鼓シミュコミュニティは「改変と共有」の文化で成り立っています。それを否定するようだったりその考えが合わないのならTJAPlayerPIのスキンを作るのは控えてください! ほかの太鼓シミュのスキンを作ってください!。
- TJAPlayerPIの開発者や貢献者は「オープンソースの信念」に反する行為を超非難します。例えばTJAPlayerPIの派生ソフトを作ってローカルで配布してネットに動画を上げて動画のコメント欄で配布してくれと言われたときに「自己満で作っているので配布しません」とか「クレクレは黙れ」とか言ってソフトやソースコードを一般配布しないのならTJAPlayerPIを使うのは控えてください!!!!。

Q1 MITライセンスだから別に良いだろ。
A1 ライセンスの関係で厳密には問題ないけど....少なくとも私はそんなことは望みません、これ以上界隈を停滞させないでください。

Q2 TJAPlayerPIの派生版でスキンの騒ぎが起こったらどうしますか?
A2 派生版の界隈に関しては管轄外です。

Q3 TJAPlayerPIのコードをほかの太鼓シミュで使うのは大丈夫ですか?
A3 問題ない。TJAPlayerPIそのものではないから管轄外。

Q4 ライセンスを変えたら良いのでは?
A4 ライセンスを変える予定はありません。コードの活用が難しくなるしライセンス変えたところで問題が解決する未来が見えない。

Q5 ちょっと厳しすぎない?
A5 あくまで私Takkkomからのお願いです。MITライセンスなのでこれらのお願いは無視しても構いません。


## 質問をする前に
質問をする前に、

1. 調べる前に考える
2. 人に聞く前に調べる
3. 過去に同じような質問がなかったか調べる
4. 使用しているパソコンの環境、どういう動作をしたら不具合を起こしたかの過程等を添えて連絡する

この4つのルールを守ってくださいますよう、よろしくお願いいたします。


## バグ報告について 
不具合を発見した場合はGitHub Issuesにてご報告いただくようお願い申し上げます。
また、情報を開発者に送信する機能は廃止しました。


## 追加機能について
「AdditionalFeatures.md」で説明いたします。


## 推奨動作環境
#### OS
* Windows 10以降のWindows (x86, x64)
* macOS 10.15 "Catalina"以降のmacOS (x64, arm64)
* デスクトップ環境構築済みの Linux ディストリビューション 最新安定版 (x64, arm64)

#### CPU
* マルチスレッド対応
* x86,x64の場合、SSE対応(BASS)


## 開発環境(動作確認環境)
#### OS
* Windows 11(Ver.24H2) (x64)

#### Editor
* Visual Studio Community 2022
* Visual Studio Code


## 開発状況
|バージョン |日付(JST) |                                        実装内容                                        |
|:---------:|:--------:|:---------------------------------------------------------------------------------------|
|Ver.2.0.0.0|??????????|未定                                                                                    |


## ライセンス関係
Fork元より使用しているライブラリ
* [bass](https://www.un4seen.com/bass.html)
* FDK21(改造しているため、FDKとは呼べないライブラリと化しています)

以下のライブラリを追加いたしました。
* [ReadJEnc](https://github.com/hnx8/ReadJEnc)
* [FFmpeg.AutoGen](https://github.com/Ruslan-B/FFmpeg.AutoGen)
* [SDL3](https://www.libsdl.org/)
* [ppy/SDL3-CS](https://github.com/ppy/SDL3-CS)
* [discord-rpc-csharp](https://github.com/Lachee/discord-rpc-csharp)
* [M+ FONTS](https://osdn.net/projects/mplus-fonts/)
* [managed-midi](https://github.com/atsushieno/managed-midi)
* [ManagedBass](https://github.com/ManagedBass/Home)
* [SkiaSharp](https://github.com/mono/SkiaSharp)
* [Tomlyn](https://github.com/xoofx/Tomlyn)
* [NAudio](https://github.com/naudio/NAudio)
* [NVorbis](https://github.com/NVorbis/NVorbis)
* [OpenAL](https://www.openal.org/)
* [Silk.NET](https://github.com/dotnet/Silk.NET)

また、フレームワークに[.NET](https://dotnet.microsoft.com/)を使用しています。

ライセンスは「ThirdPartyLicenses」に追加されています。


## FFmpegについて
`TJAPlayerPI`と同じフォルダに`FFmpeg`フォルダを作成し、  
その中にOSとTJAPlayerPIのアーキテクチャに対応したフォルダを作成し、
`TJAPlayerPI`のアーキテクチャに対応したFFmpeg 7.1バイナリ(Shared)を置くことにより、

+ FFmpegが対応している動画ファイルの再生
+ FFmpegが対応している音声ファイルの再生

ができるようになります。

### OSとTJAPlayerPIのアーキテクチャ数に対応したフォルダ名

+ Windows
  - x86   : `win-x86`
  - x64   : `win-x64`
+ macOS
  - x64   : `osx-x64`
  - arm64 : `osx-arm64`
+ Linux
  - x64   : `linux-x64`
  - arm64 : `linux-arm64`


## BASSについて
このリポジトリにはあらかじめBASSライブラリが同梱されています。

## 謝辞
このTJAPlayerPIのもととなるソフトウェアを作成・メンテナンスしてきた中でも  
主要な方々に感謝の意を表し、お名前を上げさせていただきたいと思います。

- ＦＲＯＭ様(DTXMania2の開発者)
- yyagi様(DTXManiaの開発者)
- kairera0467様(DTXManiaXG Ver.kとTJA2fPCの開発者)
- AioiLight様(TJAPlayer3の開発者)
- Mr-Ojii様(TJAPlayer3-fの開発者)

また、他のTJAPlayer関係のソースコードを参考にさせていただいている箇所がございます。  
ありがとうございます。
