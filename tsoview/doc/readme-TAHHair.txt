ダウンロードありがとうございます

●TAHHair ver 0.0.4

これはなに
髪色補完を簡単に行うためのツールです。

◆TAHHair.exe
エクスプローラからTAHHair.exeを起動します。

Loadボタン
Loadボタンを押すと、ファイル選択ダイアログが開きます。
ここで、差分を作るtahファイルを選択します。
普通はbase.tahか髪型modを選択することになります。

        エクスプローラからtahファイルを画面にドロップしてもokです。

Compressボタン
tahファイルを選択したらCompressボタンを押します。
すると、tahファイル中の髪型tsoを展開して髪色を適用しつつ差分tahを作ってくれます。
注意：base.tahには髪型tsoが大量に含まれるため、処理には時間がかかります。

あとは差分tahをarcsフォルダに移動してください。
これで髪色補完完了です！

オプション
tah version
　作成するtahのversionを指定します。
color set
　色セットを指定します。
　色セットは cols フォルダにファイルを置くと追加できます。

ライセンス
HAIR_KITフォルダは 髪色補完キットRY.ver.1.1 (mod0416) から流用しています。


仕組み
・材質名とテクスチャファイル名から材質type（Kami or Housen or Ribbon）を判定する
・材質typeに対応するcgfxファイルとテクスチャを入れ替える

ver 0.0.3 からの変更点：
・tbn 設定を外部ファイルから読む (TAHHairProcessor.xml)
・HAIR_KIT：Cgfx_ribonを追加; 基本色を追加 (tim1330)
・色番号00 icon は残す

ver 0.0.2 からの変更点：
・判定規則を外部ファイルから読む (TSOHairProcessor.xml)
・tah version を指定
・色セットを指定
・tahファイルを画面にドロップできる

ver 0.0.1 からの変更点：
・テクスチャファイル名で判定した場合でもcgfxファイルを入れ替える
・文字列に W_FacePartsA を含む場合は Housen とみなす
・typo Ribbon
