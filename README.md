# PTEditor

Editor do servidor de Priston Tale, baseado no PriTaTor 1.8.857 e migrado para
Visual Studio 2022 / .NET Framework 4.8 / VB.NET.

## Funcionalidades

| Aba | O que faz |
|-----|-----------|
| Monsters | Edita os monstros (`GameServer\Monster\*.inf`): atributos, drops e ouro. |
| Items | Edita os itens (`GameServer\OpenItem\*.txt`). |
| Maps | Edita spawns e chefes dos mapas (`GameServer\Field\*.spm`). |
| NPC Editor | Edita NPCs: nome, falas, loja e posicao no mapa. |
| Characters | Lista contas e personagens do `DataServer`, edita level, EXP, atributos, ouro e inventario e exclui personagens. |
| Item Finder | Busca itens em todos os personagens e baus, lista itens duplicados e com checksum invalido. |
| Award Items | Envia itens para o NPC de entrega (tabela `AwardItem_List` do SQL Server). |
| Options | Pasta do servidor, banco de dados, editor de texto e perfis de personagem. |

## Compilar

Abrir `PTEditor.sln` no Visual Studio 2022 (workload ".NET desktop development") ou:

```
MSBuild PTEditor.sln -p:Configuration=Release
```

A saida fica em `PTEditor\bin\Release\`. Ao copiar o PTEditor para outro lugar, leve junto a pasta `Profiles`.

## Configuracao

As opcoes ficam em `PTEditor.Config.txt`, ao lado do executavel. As principais:

```
Server-Path=C:\Servidor\
DataServer-Path=C:\Servidor\DataServer
```

- **Characters / Item Finder:** usam a pasta `DataServer` do Server Path. `DataServer-Path` so e gravado quando
  outra pasta e escolhida na aba Characters, e e descartado ao trocar o Server Path. O formato dos arquivos de cada
  source fica em `Profiles\*.ini` (`Priston` e `Custom`); por padrao o perfil e detectado automaticamente.
- **Banco de dados:** a conexao vem da secao `[Database]` do `server.ini` da pasta do servidor. Em Options, o botao
  Database abre esse arquivo e Test confere `PristonDB.AwardItem_List` (Award Items) e `ClanDB` (verificacao de clan
  ao excluir personagem).

## Cuidados

- Para editar um personagem ele precisa estar deslogado: o servidor regrava o arquivo ao sair.
- Antes de gravar, o PTEditor cria um backup (`Backup\` nas pastas do GameServer e `PTEditor_Backup\` no DataServer).
- A exclusao de personagem segue a do servidor: libera o slot da conta, guarda uma copia em `DataServer\deleted\`
  (Lv 10+) e apaga o arquivo. Personagem em clan (membro, subchefe ou lider) nao pode ser excluido, como no jogo.
- Os envios do Award Items ficam registrados em `AwardItems.log`.
