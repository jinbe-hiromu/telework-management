# [WorkScheduler] WorkScheduleServerのセットアップ及び使い方

## 概要

WorkSchedulerシステムのデータ管理サービスを提供するWorkScheduleServerに関する仕様に関して記載する。

WorkScheduleServerは、ASP.NET Core Web API とWeb App (Blazor Server)の構成で動作するASP.NET Core 6のシステムとして開発されており、Databaseとしては、SQLiteを使用、ローカルファイル（WorkScheduleDB.db）に各種データを保存、参照を行う構成である。

ソース管理先：

https://github.com/jinbe-hiromu/work-scheduler/tree/main/WorkScheduleServer



## セットアップ

### 環境変数設定

**work-scheduler\WorkScheduleServer\Set_WorkScheduleServer_Environment.bat**

上記BATファイルをコマンドプロンプトから実行してください。実行によって、WorkScheduleServer__Path変数が定義される。

SQLiteのData Source=%WorkScheduleServer__Path%server/WorkScheduleDB.db;の各自の環境においても正しく動作させるために必ず実行を行ってください。

### 実行プログラム作成

https://github.com/jinbe-hiromu/work-scheduler/blob/main/WorkScheduleServer/WorkScheduleServer.sln

Visual Studio 2022で上記ファイルを開いて、ビルドを実施してください。環境変数設定が行っていれば単体で動作します。



## 使い方

### API呼出し

最初にLoginのAPIを実施し、アクセストークンを得たから、データ管理に必要な要求（GET、POST、PUT、DELETE）行ってください。先後にLogoutを実施して終了してください。

### GUI

ブラウザーからServerのURLを指定することでGUI画面としてLoginし、データ管理が可能です。デバッグに使用してください。



### アカウント情報

デフォルトパスワードはAccount名＋"@10T" (ゼロ)

| Account名 | Password      | 備考          |
| --------- | ------------- | ------------- |
| Fukaya    | Fukaya@10T    | Role＝Manager |
| Matsubara | Matsubara@10T | Role＝Member  |
| Ishiyama  | Ishiyama@10T  | Role＝Member  |
| Yamauchi  | Yamauchi@10T  | Role＝Member  |
| Nakamura  | Nakamura@10T  | Role＝Member  |
| Touma     | Touma@10T     | Role＝Member  |
|           |               |               |
|           |               |               |
|           |               |               |

※ServerのGUIモードで、admin, admin でログインして修正は可能。



### API詳細仕様

### Login

ログイン実行

```
POST <host>/api/WorkSchedule/Login
ex. http://127.0.0.1:5000/api/WorkSchedule/Login
 [Request]
 Header {
   Content-Type: application/json; charset=utf-8
 }
 Body {
     "username" : "<ユーザー名>",
     "password" : "<パスワード>"
 }
 [Response]
 Body {
      "AccessToken" : "<アクセストークン>"
 }
```

POST時に、usernameとpasswordをBodyにJSONで指定し、応答としてAccessTokenの値を得る。

### Logout

ログアウト実行

```
 POST <host>/api/WorkSchedule/Logout
 ex. http:127.0.0.1:5000/api/WorkSchedule/Logout
 [Request]
 Header {
   Content-Type: application/json; charset=utf-8
   AccessToken: <アクセストークン>
 }
```

POST時に、AccessTokenをリクエストヘッダーに指定し、URLを呼び出す。

### Get

値の取得

```
 GET <host>/api/WorkSchedule/<year>/<month>/<day>
 ex. http:127.0.0.1:5000/api/WorkSchedule/2023/1/30
 [Request]
 Header {
   Content-Type: application/json; charset=utf-8
   AccessToken: <アクセストークン>
 }
 [Response]
 Body {
    "Date"        : "2023-01-30",
    "StartTime"   : "2023-01-30T08:40",
    "EndTime"     : "2023-01-30T17:40",
    "WorkStyle"   : "出社",　 出張,テレワーク,有休
    "WorkingPlace"   : "阿久比"　 刈谷,自宅,その他
 }
```

GET時に、AccessTokenをリクエストヘッダーに指定し、URLを呼び出す。URLのは、取得したい日付を指定する。

応答として、JSON形式で、WorkScheduleItem型のオブジェクトデータが得られる。



### Post

値の追加（更新も）

```
 POST <host>/api/WorkSchedule/<year>/<month>/<day>
 ex. http:127.0.0.1:5000/api/WorkSchedule/2023/1/30
 Header {
   Content-Type: application/json; charset=utf-8
   AccessToken: <アクセストークン>
 }
Body {
    "Date"        : "2023-01-30",
    "StartTime"   : "2023-01-30T08:40",
    "EndTime"     : "2023-01-30T17:40",
    "WorkStyle"   : "出社",　 出張,テレワーク,有休
    "WorkingPlace"   : "阿久比"　 刈谷,自宅,その他
 }
```

追加したいデータをリクエストのBodyに格納し、URLを呼び出す。URLのは、日付を指定する。



### Put

値の更新（追加も）

```
 PUT <host>/api/WorkSchedule/<year>/<month>/<day>
 ex. http:127.0.0.1:5000/api/WorkSchedule/2023/1/30
[HttpPut("{year}/{month}/{day}")]
 Header {
   Content-Type: application/json; charset=utf-8
   AccessToken: <アクセストークン>
 }
Body {
    "Date"        : "2023-01-30",
    "StartTime"   : "2023-01-30T08:40",
    "EndTime"     : "2023-01-30T17:40",
    "WorkStyle"   : "出社",　 出張,テレワーク,有休
    "WorkingPlace"   : "阿久比"　 刈谷,自宅,その他
 }
```

更新したデータをリクエストのBodyに格納し、URLを呼び出す。URLのは、日付を指定する。今の動作は、内部でPostのAPIを呼んでいるだけなので、全く同じ動作になる。



### Delete

値の削除

```
 DELETE <host>/api/WorkSchedule/<year>/<month>/<day>
 ex. http:127.0.0.1:5000/api/WorkSchedule/2023/1/30
 Header {
   Content-Type: application/json; charset=utf-8
   AccessToken: <アクセストークン>
 }
```

URLで対象のDateが指定されるので、アクセストークンだけ指定してDELETEリクエストを行えば、削除が実行される。



## 補足

### API実装クラス

work-scheduler\WorkScheduleServer\server\Controllers\WorkScheduleController.cs

### DBデータモデル

#### InputDetailsContact相当

work-scheduler\WorkScheduleServer\server\Models\WorkScheduleItem.cs

#### InputDetailsContact+ID(PrimaryKey)

work-scheduler\WorkScheduleServer\server\Models\WorkScheduleDB\WorkSchedule.cs

#### AccountInfo定義

Username, Passwordの指定用クラス

POST <host>/api/WorkSchedule/Loginで指定するオブジェクト定義

WorkScheduleServer.Controllers.WorkScheduleController.AccountInfo

##  