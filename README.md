# Fcg.Contracts

Pacote NuGet de **contratos de evento de integração** compartilhados entre os serviços do FCG
(FIAP Cloud Games). Contém apenas **records C# puros** de evento — sem lógica, sem dependências de
runtime, sem topologia de transporte (exchange/fila vivem na configuração de bus de cada serviço).

- **TargetFramework:** `net10.0`
- **PackageId:** `Fcg.Contracts` · **namespaces:** `Fcg.Contracts.Events` (eventos) · `Fcg.Contracts.Enums` (enums de apoio)
- **Feed:** GitHub Packages (GHCR) de `reinaldogez`

## Contratos disponíveis

| Contrato | Publicado por | Consumido por | Notas |
|----------|---------------|---------------|-------|
| `UserCreatedEvent` | `fcg-identity` | `fcg-notifications` | usuário criado |
| `OrderPlacedEvent` | `fcg-catalog` | `fcg-payments` | pedido pendente; carrega `GameName`/`Price` (fat event) |
| `PaymentProcessedEvent` | `fcg-payments` | `fcg-catalog`, `fcg-notifications` | resultado do pagamento; traz `PaymentId`, `Status` (`PaymentStatus`) e `RejectionReason?` |
| `PaymentStatus` (enum) | — | — | `Approved = 1`, `Rejected = 2` |

## Como consumir

No repositório que consome os contratos, garanta um `nuget.config` na raiz com o source do feed
público (o mesmo arquivo deste repo):

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
    <add key="github-fcg" value="https://nuget.pkg.github.com/reinaldogez/index.json" />
  </packageSources>
</configuration>
```

E adicione o pacote:

```bash
dotnet add package Fcg.Contracts
```

O feed do GitHub Packages **exige autenticação mesmo para package público** — o package precisa estar
público **e** o consumidor precisa de um token com escopo `read:packages`. Em **CI (GitHub Actions)** o
restore usa o `GITHUB_TOKEN` do runner (`permissions: packages: read`), sem PAT. Em **build local**,
configure um PAT com `read:packages` no `nuget.config` *user-level* — o PAT não deve ser commitado no
`nuget.config` versionado.

Como a fonte `github-fcg` já existe no `nuget.config` deste repo, use `update source` para anexar a
credencial a ela (`--username` é o seu login do GitHub; o que autentica de fato é o PAT em `--password`):

```powershell
dotnet nuget update source github-fcg `
  --username <seu-usuário-do-github> `
  --password <PAT> `
  --store-password-in-clear-text
```

Se a fonte ainda não existir no seu ambiente, use `add source` informando também a URL do feed:

```powershell
dotnet nuget add source https://nuget.pkg.github.com/reinaldogez/index.json `
  --name github-fcg `
  --username <seu-usuário-do-github> `
  --password <PAT> `
  --store-password-in-clear-text
```

Uso típico do contrato:

```csharp
using Fcg.Contracts.Events;

var evt = new UserCreatedEvent
{
    UserId = Guid.NewGuid(),
    Name = "Ada Lovelace",
    Email = "ada@example.com",
    OccurredAt = DateTimeOffset.UtcNow
};
```

Eventos que usam enums de apoio trazem também o namespace `Fcg.Contracts.Enums`.
Por exemplo, o `PaymentProcessedEvent` carrega o `PaymentStatus` e o
`RejectionReason` anulável (presente só quando `Rejected`):

```csharp
using Fcg.Contracts.Events;
using Fcg.Contracts.Enums;

var evt = new PaymentProcessedEvent
{
    OccurredAt = DateTimeOffset.UtcNow,
    PaymentId = Guid.NewGuid(),
    OrderId = Guid.NewGuid(),
    UserId = Guid.NewGuid(),
    UserEmail = "ada@example.com",
    UserName = "Ada Lovelace",
    GameId = Guid.NewGuid(),
    GameName = "Half-Life",
    Price = 49.90m,
    Status = PaymentStatus.Rejected,
    RejectionReason = "Cartão recusado"
};
```

## Versionamento

A versão do pacote é controlada por **bump manual SemVer** no `<Version>` do `.csproj`:

- **patch** — mudança interna sem efeito de wire (XML doc, comentário, refactor).
- **minor** — adição retrocompatível: **record de evento novo** ou campo opcional novo.
- **major** — mudança breaking. Pela regra de **imutabilidade do wire** (um record publicado nunca
  é editado de forma breaking — breaking vira um record novo, ex. `...V2`), isto quase nunca ocorre.

Para publicar: editar `<Version>`, commitar e fazer push na branch principal. O CI faz `pack` +
`push` com `--skip-duplicate` (se a versão já existe no feed, o push é ignorado graciosamente).

> **`EventVersion` (campo do record) ≠ versão do pacote NuGet.** São eixos independentes:
> `EventVersion` muda só em evolução de schema do evento; a versão NuGet versiona o artefato.
