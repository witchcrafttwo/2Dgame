# 2D Action Starter

Unity 6000系の2Dアクション試作用テンプレートです。まずは「横移動、ジャンプ、足場、敵、ダメージ、チェックポイント、落下リスポーン、ゴール」までを小さく作れる状態にしています。

## 使い方

1. Unityで `C:\Users\yunre\2Dgame` を開く。
2. メニューから `2D Action Starter > Build Starter Scene` を実行する。
3. 生成されたシーンを保存する。
4. Playを押して、左右移動とジャンプを確認する。

## 操作

- 移動: `A / D` または `Left / Right`
- ジャンプ: `Space / W / Up`
- ゲームパッド: 左スティック移動、下ボタンジャンプ

## 追加した型

- `GameManager2D`: プレイヤー登録、チェックポイント、リスポーン、ポーズ管理
- `PlayerController2D`: 2D横移動、ジャンプ、ジャンプ猶予、入力バッファ
- `PlayerHealth2D`: 体力、無敵時間、死亡時リスポーン
- `CameraFollow2D`: プレイヤー追従カメラ
- `SimpleEnemyPatrol2D`: 左右に巡回する敵
- `DamageOnTouch2D`: 触れたプレイヤーへダメージ
- `Checkpoint2D`: リスポーン地点更新
- `KillZone2D`: 落下時のリスポーン
- `LevelGoal2D`: ゴール到達イベント

## 次に作ると良さそうなもの

- プレイヤーのアニメーション
- 攻撃アクション
- タイルマップでのステージ制作
- UIの体力表示
- 敵の種類追加
- 効果音とBGM
