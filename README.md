# 2D Action Starter

Unity 6000系の2Dアクション試作用テンプレートです。まずは「横移動、ジャンプ、攻撃、敵撃破、足場、ダメージ、チェックポイント、落下リスポーン、ストーリー会話、ゴール」までを小さく作れる状態にしています。

## 使い方

1. Unityで `C:\Users\yunre\2Dgame` を開く。
2. メニューから `2D Action Starter > Build Starter Scene` を実行する。
3. 生成されたシーンを保存する。
4. Playを押して、左右移動とジャンプを確認する。

## Assetのキャラを動かす

1. Projectウィンドウで動かしたいキャラPrefabをクリックする。
2. メニューから `2D Action Starter > Spawn Selected Asset As Player` を実行する。
3. Playを押して操作する。

このメニューは、選択したAssetをシーンに生成して、`Rigidbody2D`、`CapsuleCollider2D`、`PlayerController2D`、`PlayerAttack2D`、`PlayerHealth2D`、`GroundCheck`、`AttackPoint` を追加します。古い仮Playerの操作スクリプトは自動で止め、カメラ追従も新しいキャラへ切り替えます。

すでにシーン上に置いたキャラを動かしたい場合は、そのキャラを選択して `2D Action Starter > Make Selected Character Playable` を実行します。

## 操作

- 移動: `A / D` または `Left / Right`
- ジャンプ: `Space / W / Up`
- 攻撃: `J / F / 左クリック`
- 会話送り: `Enter / Space / E / 左クリック`
- ゲームパッド: 左スティック移動、下ボタンジャンプ

## 追加した型

- `GameManager2D`: プレイヤー登録、チェックポイント、リスポーン、ポーズ管理
- `PlayerController2D`: 2D横移動、ジャンプ、ジャンプ猶予、入力バッファ
- `PlayerAttack2D`: 近接攻撃、攻撃範囲、クールダウン
- `PlayerHealth2D`: 体力、無敵時間、死亡時リスポーン
- `CameraFollow2D`: プレイヤー追従カメラ
- `EnemyHealth2D`: 敵HP、被ダメージ、撃破処理
- `SimpleEnemyPatrol2D`: 左右に巡回する敵
- `HitFlash2D`: 被弾時の色変化
- `DamageOnTouch2D`: 触れたプレイヤーへダメージ
- `Checkpoint2D`: リスポーン地点更新
- `KillZone2D`: 落下時のリスポーン
- `LevelGoal2D`: ゴール到達イベント
- `StoryManager2D`: 会話UI、会話中ポーズ、会話送り
- `StoryTrigger2D`: プレイヤー接触でストーリー再生

## 次に作ると良さそうなもの

- プレイヤーのアニメーション
- 攻撃エフェクト
- タイルマップでのステージ制作
- UIの体力表示
- 敵の種類追加
- 効果音とBGM
