◆TPOEditor.exe
エクスプローラからTPOEditor.exeを起動します。
マイドキュメントのカス子セーブ (system.tdcgsav.png) が自動的に読み込まれます。

	参考：png tso tmoファイルを画面上にドラッグ＆ドロップ (d&d) すると追加できます。

注意：
・TPOEditor.exeと同じフォルダにtoonshader.cgfxが必要です。
・必ずpng tso tmoファイルをバックアップしてください。

操作方法
操作方法は基本的にTSOViewと同じです（ただしキーボードでの操作は無効）。

マウス
　左ボタンを押しながらドラッグ：X/Y軸方向に回転
　中央ボタンを押しながらドラッグ：X/Y軸方向に移動
　右ボタンを押しながらドラッグ：Z軸方向に移動
　Ctrlキーを押しながらクリック：ライト方向を指定

●体型リスト
画面左上には体型リストがあります。
選択すると、ボーンリストが切り替わります。

●ボーンリスト
画面左中央にはボーンリストがあります。
選択すると、変形操作リストと変形グリッドが切り替わります。

●変形操作リスト（参照のみ）
画面右上には変形操作リストがあります。
指定ボーンに対する変形操作を参照できます。
Type：変形操作タイプ
	Scale：拡大
	Scale1：拡大（ただし子の拡大率を維持する）
	Scale0：縮小
	Rotate：回転
	Move：移動
X, Y, Z：X軸, Y軸, Z軸に対する値を残照できます。

●変形グリッド
画面右中央には変形グリッドがあります。
指定ボーンに対する変形操作を編集できます。
Type：変形操作タイプ
	Scale：拡大縮小
	Rotate：回転
	Move：移動
X, Y, Z：X軸, Y軸, Z軸に対する値を編集できます。
	編集するには、ダブルクリックまたはF2キーを押します。
	また、マウスホイールを回転させると、値が増減します。

inverse scale on children：
	チェックを付けると、全ての子ボーンに対して縮小操作を追加します。
	これで子の拡大率を維持できます。

●Saveボタン
Saveボタンを押すと、体型ファイルに体型情報を保存します（Proportion/*.cs）。


ver 0.2.8 からの変更点：
・変形グリッドで並び替えできてしまうバグを修正
