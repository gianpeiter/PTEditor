# PTEditor 1.0

Editor de GameServer do Priston Tale (monstros, itens, mapas de spawn e NPCs), baseado no
PriTaTor 1.8.857 (fonte decompilado, VS 2003) para **Visual Studio 2022 / .NET Framework 4.8 / VB.NET**.

## Compilar

Abrir `PTEditor.sln` no VS 2022 (workload ".NET desktop development") e compilar, ou:

```
MSBuild PTEditor.sln -p:Configuration=Release
```

Saida em `PTEditor\bin\<Configuration>\`. Os formularios abrem no designer do VS normalmente.

## Estrutura

| Pasta | Conteudo |
|-------|----------|
| `State\AppState.vb` | Pastas/padroes do servidor, encoding, listas carregadas (`Monsters`, `Items`, `Maps`, `Npcs`, `SpcFiles`) e tag de idioma dos `.zhoon`. |
| `State\ConfigFile.vb` | Leitura/gravacao de `PTEditor.Config.txt` (filtros dos combos e opcoes). |
| `Data\GameFields.vb` | Chaves coreanas dos arquivos (`*레벨`, `*아이템`, ...), legiveis no codigo. |
| `Data\Models.vb` | `MonsterFile`, `ItemFile`, `MapFile`, `NpcFile`, `BinaryDataFile` (linhas em memoria). |
| `Data\MonsterData.vb` | Carga, campos, drops, ouro, filtros e gravacao dos `Monster\*.inf`. |
| `Data\ItemData.vb` | Carga, busca e gravacao dos `OpenItem\*.txt`. |
| `Data\MapData.vb` | Carga e gravacao dos `Field\*.spm` (spawns e chefes). |
| `Data\NpcData.vb` | `NPC\*.NPC` (nome, falas, loja) e registros binarios dos `Field\*.spc` (504 bytes cada). |
| `Data\ZhoonFile.vb` | Arquivos `.zhoon`: aceita `*X_NAME`/`*X_CHAT` de qualquer idioma (o original so lia `*J_`). |
| `Data\BackupHelper.vb` | Backups em `<pasta>\Backup\` (`Default_<arquivo>` na primeira vez, depois com data/hora). |
| `Data\Character\` | Personagens, contas e baus do `DataServer` (ver secao abaixo). |
| `Helpers\` | Log de erros, dialogos, I/O com encoding, avaliacao de formulas, abertura de arquivos externos. |
| `Forms\` | `MainEditor` (janela principal), `CharacterEditor` (aba Characters), `ItemFinder` (aba Item Finder), `ExpChanger`, `GoldChanger`, `ItemSearch`, `ItemDistributor`. |
| `Maps\*.bmp` | Miniaturas dos mapas para o editor de NPC (copiadas para a saida). |

Os formularios so cuidam da interface; leitura/gravacao fica em `Data\`.

## Personagens (abas Characters e Item Finder)

Le `DataServer\userdata\<n>\*.dat`, `userinfo\<n>\*.dat` e `warehouse\<n>\*.war` (`<n>` = soma dos bytes do nome,
a..z em maiusculas, modulo 256 - `GetUserCode` em `Servidor\record.cpp`). A pasta fica em `PTEditor.Config.txt`:

```
DataServer-Path=C:\Servidor\DataServer
```

| Arquivo | Conteudo |
|---------|----------|
| `RecordLayout.vb` | Offsets dos dois layouts conhecidos: `Servidor` (source atual, itens em `0x718`) e `Classic` (PTClassic, itens em `0x6F0`). Detectado pelo cabecalho de cada arquivo. |
| `CharacterFile.vb` | `TRANS_RECORD_DATA`: nome, classe, level, EXP 64 bits (`Exp` + `Exp_High`), atributos, ouro, skills, itens; grava recomprimindo os itens. |
| `ItemCompression.vb` | `EecodeCompress`/`DecodeCompress` (byte com bit 7 = sequencia de zeros, senao bytes literais). Recomprimir reproduz os bytes do jogo. |
| `ItemChecksum.vb` | `dwChkSum` (conferido pelo cliente ao logar) e `dwTime` (`rsGetItem_ServerForm`, conferido pelo servidor ao vender/trocar). |
| `CharacterRules.vb` | O que o servidor faz ao carregar: level corrigido pela EXP (`LevelTable.vb`, copia de `LevelTable.h`), pontos livres recalculados (`ReformCharStatePoint`), reset de atributos abaixo de -10. |
| `WarehouseFile.vb` | `.war`: um bloco comprimido com 100 `sITEM`, ouro gravado como valor + 2023, checksum do bau. Somente leitura. |
| `CharacterIndex.vb` | Varre o DataServer; arquivos ilegiveis entram na lista com o motivo, sem interromper a carga. |

Ao salvar: confirmacao com as diferencas e avisos, copia em `<DataServer>\PTEditor_Backup\`, pontos livres gravados
como o servidor calcularia e `LastMoeny` zerado (se > 0 o jogo usaria `LastMoeny - 1` como ouro). O personagem
precisa estar deslogado: o servidor regrava o arquivo ao sair.

Pendente: conferir o layout `Servidor` e o formato do bau com arquivos reais deste servidor (os testes usaram
personagens do PTClassic e um bau gerado a partir do fonte).

## Encoding dos arquivos do servidor

Os arquivos do GameServer sao CP949, mas o PriTaTor sempre os leu e gravou como CP1252
(`AppState.FileEncoding`). Isso foi mantido: a gravacao reproduz os bytes originais e a leitura
seguida de gravacao sem alteracoes e identica byte a byte (quebra de linha final inclusive).
`GameFields.Key()` converte o texto coreano escrito no codigo para a forma CP1252 que aparece em memoria.

## Tag de idioma dos .zhoon

O servidor le `*B_NAME`/`*B_CHAT`, `*J_NAME`, `*E_NAME`... conforme o `#ifdef _LANGUAGE_*` com que foi
compilado (ver `Servidor\fileread.cpp`). O editor aceita qualquer tag ao ler; para gravar usa a tag mais
frequente encontrada nos arquivos carregados, ou a definida em `PTEditor.Config.txt`:

```
Name-Tag=B
```

## Diferencas em relacao ao 1.8.857 original

Corrigidos:

- Erros de compilacao do fonte decompilado (`(0 To .,0 To .)`, `Catch ... When (?)`, `Path.GetFileName` em string, `FileSystem.WriteLine`).
- "New line" na lista de drops embutia CRLF dentro de uma linha do arquivo.
- "Delete line" na lista de drops nao apagava a linha do arquivo (duplicava a ultima).
- Campo SP Block mostrava um caractere coreano a mais (tamanho da chave errado) e o gravava de volta.
- SP HP Regen era exibido/gravado no campo SP Def (os dois ficavam com o mesmo valor ao salvar).
- "req HP" (`*건강`) ignorava a edicao do usuario ao salvar.
- "Monster not in Map" listava todos os mapas.
- "Drop Count" usava o indice da lista como indice do monstro (alterava monstros errados).
- "Remove item" fazia `ToUpper` na linha inteira, corrompendo o marcador de ouro (`돈`).
- Level exibido ficava desatualizado apos salvar (cache so era refeito no Reload).
- "Add new NPC" sobrescrevia o slot 0 quando nao havia slot vazio e falhava ao sobrescrever arquivo existente.
- Validacoes do editor de NPC (valores nao numericos, ID alto) agora impedem a gravacao em vez de so avisar.
- Caminhos com espaco ao abrir arquivos no editor externo (argumento sem aspas).
- Resultado de formula de EXP truncado para Int32 (overflow virava 0).
- Ultimo item nunca aparecia na busca por nome.
- Lista de `.inx` duplicava a cada Reload.
- Arquivos com mais de 499 linhas eram truncados.
- `ItemDistributor` quebrava na abertura se `DataServer\userinfo` nao existisse; nomes de personagem vinham com `\0`.

Mantidos como no original (comportamento do formato, nao bug): normalizacao de tabs nas linhas de drop editadas,
`*이동속도`/`**이동속도` mapeados para "SP Run"/"Run", valor "Normal" de raca nao altera a linha.
