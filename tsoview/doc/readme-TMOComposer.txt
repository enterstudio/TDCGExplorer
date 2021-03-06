ダウンロードありがとうございます

●TMOComposer ver 0.3.7

これはなに
ポーズからモーションを簡単に作成するツールです。

動作環境
・.NET Framework 3.5
・DirectX 9.0c
・toonshaderに対応したGPU
・ICSharpCode.SharpZipLib.dll (同梱)
・CSScriptLibrary.dll (同梱)

使い方
◆TMOComposer.exe
エクスプローラからTMOComposer.exeを起動します。

初めて起動すると、マイドキュメントにあるかす子セーブ (system.tdcgsav.png) が読み込まれます。

[Get poses] ボタンを押すと、ポーズフォルダにあるpngが読み込まれ、
画面右側のポーズリストにポーズ画像が表示されます。

ポーズ画像をダブルクリックすると、画面左側のポーズスタックに入ります。
[ Animate ] ボタンを押すと、スタック上のポーズを補間・連結したモーションが作成されます。
モーションは自動的に実行されます。

◆出力ファイル
[ Animate ] ボタンを押したタイミングで、下記のファイルが作成されます。
・スタックの情報：PngSave.xml に保存されます。
・スタック上のポーズ：ポーズフォルダの中にpngファイルとして保存されます。
・連結モーション：out-000.tmo, out-001.tmo, ... に保存されます（数字はフィギュア番号です）。

◆セーブスタック
・SaveFile：セーブファイル名

[ Rec ] ボタンを押すと、画面キャプチャを行います。キャプチャ画像は snapshots フォルダに保存されます。
[ Add ] ボタンを押すと、セーブファイルを選択して、フィギュアを追加できます。

	[Get saves] ボタンを押すと、セーブフォルダにあるpngが読み込まれ、
	セーブリストにセーブ画像が表示されます。
	
	参考：TAHBackgound.exeを使うと既存の背景をpngとして取り込めます。

[ Del ] ボタンを押すと、選択中のフィギュアを削除します。

スタックの行を選択すると、フィギュアを指定でき、ポーズスタックが切り替わります。

◆ポーズスタック
・PoseFile：ポーズファイル名
・Length：補間長（フレーム数）
・FaceFile：表情ポーズファイル名
・Accel：補間速度係数

[ Copy] ボタンを押すと、選択中のポーズ設定を複写します。
[ Flip] ボタンを押すと、選択中のポーズを左右反転します。
[ Up  ] ボタンを押すと、選択中のポーズ設定が一つ上に移動します。
[ Down] ボタンを押すと、選択中のポーズ設定が一つ下に移動します。
[ Del ] ボタンを押すと、選択中のポーズ設定を削除します。

スタックの行をダブルクリックすると、ポーズ設定を編集できます。

なお、ポーズ画像を選択してCtrl+Cキーを押すと、クリップボードにポーズファイル名が格納されます。
これを使って、スタックのPoseFileを直接変更することも可能です。

◆ポーズ編集
ポーズスタックの行を選択すると、モーションは停止します。
フィギュアにボーンを操作するハンドル（丸いしるし）が現れます。
これを使うと、ポーズを編集できます。

赤丸：左右にドラッグするとX軸回転を行います。
緑丸：左右にドラッグするとY軸回転を行います。
青丸：左右にドラッグするとZ軸回転を行います。

シフトキーを押した状態で腰ボーンをドラッグすると、フィギュアを移動できます。
シフトキーを押した状態で手足のボーンをドラッグすると、逆運動学 (CCD IK) による最適解を得ます。
ドラッグ位置に現れる黄色の丸はIKの目標を示します。

◆オプション
・Limit Rotation：回転角制限
回転角制限を有効にすると、関節の回転角を制限します。

	参考：回転角の範囲は angle-GRABIA-zxy.xml, angle-GRABIA-xyz.xml で設定しています。

・Grounded：接地IK
接地IKを有効にすると、腰ボーンをドラッグしたときに足先に対するIKを行います。


◆TMOComposerConfig.exe
TMOComposerConfig.exeを使ってconfig.xmlを修正することで、基本設定を変更できます。
・ClientSize：画面サイズ
・RecordStep：画面キャプチャのステップ数
・SavePath：セーブファイルを読み込むフォルダ
・PosePath：ポーズファイルを読み込むフォルダ
・FacePath：表情ポーズファイルを読み込むフォルダ


制限：
・体型変更している場合はうまくいきません。
・旧セーブを追加すると、セーブスタックの行とフィギュアとの対応にずれが生じます（バグ）。

注意：
・TMOComposer.exeと同じフォルダにtoonshader.cgfxが必要です。
・必ずpngファイルをバックアップしてください。


ver 0.3.1 からの変更点：
・Flipボタンを追加

ver 0.0.8 からの変更点：
・sources: TSOView, TMOComposer, TMOProportion を統合
・TMOProportion で作成した体型を自動適用（モーション再生時のみ）

ver 0.0.7 からの変更点：
・PNG書き出し
・ポーズリストは作成日時順

ver 0.0.6 からの変更点：
・回転角制限
・接地IK
・ポーズ複写

ver 0.0.5 からの変更点：
・モーション停止
・ポーズ編集
・フィギュア移動

ver 0.0.4 からの変更点：
・PosePath, FacePathが効かないバグを修正

ver 0.0.3 からの変更点：
・補間速度係数

ver 0.0.2 からの変更点：
・表情の置き換え
・ポーズを1フレームに強制切り詰め
・フォルダ指定

ver 0.0.1 からの変更点：
・up固定カメラ
・複数フィギュア
・画面キャプチャ
・画面サイズ指定
