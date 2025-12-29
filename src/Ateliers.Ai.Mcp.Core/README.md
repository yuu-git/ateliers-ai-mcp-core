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


