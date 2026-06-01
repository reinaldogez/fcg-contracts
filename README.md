# Fcg.Contracts

Pacote NuGet de **contratos de evento de integração** compartilhados entre os serviços do FCG
(FIAP Cloud Games). Contém apenas **records C# puros** de evento — sem lógica, sem dependências de
runtime, sem topologia de transporte (exchange/fila vivem na configuração de bus de cada serviço).

- **TargetFramework:** `net10.0`
- **PackageId:** `Fcg.Contracts` · **namespace:** `Fcg.Contracts.Events`
- **Feed:** GitHub Packages (GHCR) de `reinaldogez`

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
configure um PAT com `read:packages` no `nuget.config` *user-level* (`dotnet nuget add source ... --username
<seu-usuário> --password <PAT> --store-password-in-clear-text`) ou via variável de ambiente — **nunca**
comite o PAT no `nuget.config` versionado.

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
