# Ateliers AI MCP Core

Atelirs MCP エコシステム[Model Context Protocol (MCP)](https://modelcontextprotocol.io/)を実装を構築するためのコアライブラリです。

## インストール

```bash
dotnet add package Ateliers.Ai.Mcp.Core
```

## 概要

このパッケージは、MCP実装を構築するための基本インターフェースとユーティリティを提供します。

### 主な機能

- **IAteliersMcpInfo インターフェース** - MCP情報の標準化されたアクセス
- **アセンブリメタデータの自動化** - バージョン情報、ライセンス情報の自動取得
- **依存性注入対応** - .NET DIコンテナとシームレスに統合
- **.NET 10.0 サポート** - 最新の.NET機能を活用

## 使用方法

```csharp
using Ateliers.Ai.Mcp.Core;

// IAteliersMcpInfo インターフェースを実装するクラスを作成
public class MyMcpService : IAteliersMcpInfo
{
    public string Name => "MyMcpService";
    public string Version => "1.0.0";
    // ... その他の実装
}
```

---

## ロガー使用方法

Ateliers.Ai.Mcp.Core は、MCP ロギングポリシーに準拠したロギング機能を提供します。
詳細は [MCP Logging Policy](/docs/LoggingPolicy.md) を参照してください。

### 最小利用例（Core / Tool 側）
```csharp
var logger = new FileMcpLogger(new McpLoggerOptions
{
    MinimumLevel = McpLogLevel.Information
});

logger.Info("MCP.Start tool=notion.sync");

try
{
    // 処理
}
catch (Exception ex)
{
    logger.Error("MCP.Failed", ex);
    throw;
}
```

### 使い方（即実戦）

```csharp
var options = new McpLoggerOptions
{
    MinimumLevel = McpLogLevel.Information,
    LogDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ateliers", "mcp", "logs"
    )
};

IMcpLogger logger = new CompositeMcpLogger(new IMcpLogger[]
{
    new ConsoleMcpLogger(options),
    new FileMcpLogger(options)
});

logger.Info("MCP.Start tool=notion.sync");
```

```csharp
using var scope = new McpExecutionContextScope("notion.sync");

logger.Info("MCP.Start");

await RunAsync();

logger.Info("MCP.Success");
```

### Production 利用例

```csharp
services.AddMcpLogging(logging =>
{
    logging
        .SetMinimumLevel(McpLogLevel.Information)
        .AddConsole()
        .AddFile();
});
```

### UnitTests 利用例

```csharp
var services = new ServiceCollection();

InMemoryMcpLogger testLogger;

services.AddMcpLogging(logging =>
{
    logging
        .SetMinimumLevel(McpLogLevel.Debug)
        .AddInMemory(out testLogger);
});

var provider = services.BuildServiceProvider();

var logger = provider.GetRequiredService<IMcpLogger>();

logger.Info("Test message");

Assert.Single(testLogger.Entries);
Assert.Equal("Test message", testLogger.Entries[0].Message);
```

#### CompositeMcpLogger

複数のロガーを組み合わせて使用します。

✔ ロガー同士を独立させている
- File が死んでも Console は生きる
- 将来 Serilog を足しても既存コード不変

✔ 例外を握りつぶす理由
- MCP は「ログが目的のプロセス」ではない
- ログ失敗で Tool 実行が止まるのは最悪

### Log Retention

MCP はログファイルを無制限に保持しません。

ログは起動時に自動的にクリーンアップされ、以下の保持期間を超えたファイルは削除されます。

| Category | Retention |
|--------|-----------|
| Trace  | 1–3 days |
| Debug  | 3–7 days |
| Info   | 14 days |
| Error  | 90 days |

Retention 処理は MCP 起動時に一度だけ実行されます。

Retention 判定にはファイルの LastWriteTime (UTC) を使用します。
これはタイムゾーンや夏時間の影響を避けるためです。

---

## McpLogReader

MCP ログファイルを読み取るためのユーティリティクラスです。

### DI 登録（例）

FileMcpLogReader を DI コンテナに登録する例：

```csharp
services.AddSingleton<IMcpLogReader>(
    _ => new FileMcpLogReader(
        Path.Combine(AppContext.BaseDirectory, "logs")
    ));
```

InMemory を使う場合:

```csharp
services.AddSingleton<InMemoryMcpLogReader>();
services.AddSingleton<IMcpLogReader>(
    sp => sp.GetRequiredService<InMemoryMcpLogReader>());
```

---

## Ateliers AI MCP エコシステム

このパッケージは Ateliers AI MCP エコシステムの一部です：

- **Ateliers.Ai.Mcp.Core** (このパッケージ) - 基本インターフェースとユーティリティ
- **Ateliers.Ai.Mcp.Services** - サービス層の抽象化
- **Ateliers.Ai.Mcp.Tools** - MCP ツール実装
- **Ateliers.Ai.Mcp.Servers** - MCP サーバー実装

## ドキュメント

詳細なドキュメント、使用例、ガイドについては [ateliers.dev](https://ateliers.dev) をご覧ください。

## サポート

- **ドキュメント**: [ateliers.dev](https://ateliers.dev)
- **GitHub**: [yuu-git/ateliers-ai-mcp-core](https://github.com/yuu-git/ateliers-ai-mcp-core)
- **Issues**: [GitHub Issues](https://github.com/yuu-git/ateliers-ai-mcp-core/issues)
- 
## ステータス

⚠️ **開発版（v0.x.x）** - 破壊的な変更がされる可能性があります。安定版 v1.0.0 は以降は、極端な破壊的変更はしない予定です。

## ライセンス

MIT License - 詳細は [LICENSE](https://github.com/yuu-git/ateliers-ai-mcp-core/blob/master/LICENSE) ファイルをご覧ください。


