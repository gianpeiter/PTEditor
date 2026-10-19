# PTEditor

Editor do servidor de Priston Tale, baseado no PriTaTor 1.8.857 e migrado para
Visual Studio 2022 / .NET Framework 4.8 / VB.NET.

## Funcionalidades

| Aba | O que faz |
|-----|-----------|
| Monsters | Edita os monstros (`GameServer\Monster\*.inf`): atributos, drops e ouro. |
| Items | Edita os itens (`GameServer\OpenItem\*.txt`). |
| Maps | Edita spawns e chefes dos mapas (`GameServer\Field\*.spm`). |
| Spawn Point | Adiciona, move e remove os pontos de nascimento (`GameServer\Field\*.spp`) sobre a vista de cima do mapa (`Field\*.smd`). |
| NPC Editor | Edita NPCs: nome, falas, loja e posicao no mapa. |
| Characters | Lista contas e personagens do `DataServer`, edita level, EXP, atributos, ouro, inventario, rank de classe (libera as skills no Skill Master) e quests, e exclui personagens. |
| Item Finder | Busca itens em todos os personagens e baus, lista itens duplicados e com checksum invalido. |
| Award Items | Envia itens para NPCs com `*AwardItem` (tabela `AwardItem_List` do SQL Server). |
| PostBox | Envia itens para NPCs com `*경품배달`, como o Item Distributor (arquivos `PostBox\<n>\<conta>.dat`). |
| Options | Pasta do servidor, banco de dados, editor de texto e perfis de personagem. |

## Compilar

Abrir `PTEditor.sln` no Visual Studio 2022 (workload ".NET desktop development") ou:

```
MSBuild PTEditor.sln -p:Configuration=Release
```

A saida fica em `PTEditor\bin\Release\`. Ao copiar o PTEditor para outro lugar, leve junto a pasta `PTEditor` (com `Maps`, `Profiles` e `Logs`).

## Configuracao

Tudo que o programa gera fica na pasta `PTEditor`, criada ao lado do executavel. As opcoes ficam em `PTEditor\PTEditor.Config.txt`. As principais:

```
Server-Path=C:\Servidor\
DataServer-Path=C:\Servidor\DataServer
```

- **Characters / Item Finder:** usam a pasta `DataServer` do Server Path. `DataServer-Path` so e gravado quando
  outra pasta e escolhida na aba Characters, e e descartado ao trocar o Server Path. O formato dos arquivos de cada
  source fica em `PTEditor\Profiles\*.ini` (`Priston` e `Custom`); por padrao o perfil e detectado automaticamente.
- **Banco de dados:** a conexao vem da secao `[Database]` do `server.ini` da pasta do servidor. Em Options, o botao
  Database abre esse arquivo e Test confere `PristonDB.AwardItem_List` (Award Items) e `ClanDB` (verificacao de clan
  ao excluir personagem).

## Entrega de itens: Award Items e PostBox

O servidor tem dois sistemas de entrega pelo NPC, e cada instalacao usa um, o outro ou os dois. Para saber qual, abra
o `.npc` do NPC de entrega (ex.: `GameServer\NPC\itempost.npc`, o Item Distributor):

| Linha no `.npc` | Onde ficam os itens | Aba |
|-----------------|---------------------|-----|
| `*AwardItem` | Tabela `PristonDB.dbo.AwardItem_List` (SQL Server) | Award Items |
| `*경품배달` | Arquivos `PostBox\<n>\<conta>.dat` na pasta do servidor | PostBox |

As duas linhas podem ficar no mesmo NPC: um clique atende os dois sistemas. O `.npc` e lido quando o servidor inicia.

### PostBox

- Um arquivo por conta: `<Server-Path>\PostBox\<n>\<conta>.dat`, onde `<n>` e a soma dos bytes do nome da conta
  (0..255), a mesma regra das pastas do `DataServer`. Cada linha e um item:
  `<personagem ou *>  <codigo>  <spec>  "<mensagem>"`.
- **Destinatario:** escolha um personagem do DataServer carregado (a conta vem dele). Por padrao so esse personagem
  retira o item; com "Any character of the account (*)" qualquer personagem da conta retira.
- **Spec:** em armas e armaduras e a classe do item (preenchida com a classe do personagem; 0 = aleatoria). Em
  pocoes e a quantidade. O valor fica fixo no envio.
- **Mensagem:** texto da caixa de confirmacao no jogo (ate 127 caracteres).
- O NPC oferece um item por vez; o cliente confere espaco e peso antes de aceitar, entao o jogador pode retirar depois.
  O arquivo aceita ate 500 linhas por conta.
- **Show pending items** lista o arquivo da conta (em vermelho, linhas que o NPC nunca oferece) e **Remove selected**
  apaga so as linhas escolhidas, sem alterar as demais.
- Os itens tambem podem ser enviados pela aba Items, botao direito > Send to PostBox.

## Spawn Point

- O `.spp` guarda ate 200 pontos (`state`, `x`, `z`, 12 bytes cada), a mesma "flag" que o GM poe no jogo. Nao tem altura
  nem monstro: quem nasce, quantos e a que ritmo vem do `.spm` (aba Maps).
- Clicar no mapa adiciona um ponto no primeiro slot livre (como o servidor); clicar num ponto o seleciona e arrastar o move.
  Botao direito arrasta a vista, a roda da zoom e Delete remove o ponto selecionado. Nada e gravado ate clicar em Save.
- Ctrl+Z desfaz e Ctrl+Y (ou Ctrl+Shift+Z) refaz, ate 100 passos por mapa; arrastar um ponto conta como um passo. A pilha
  zera ao trocar de mapa ou recarregar, e desfazer ate o estado salvo tira o `*` do Save.
- O mapa e lido do `.smd` em `<Server-Path>\Field\` (procurado pelo nome base, ex.: `ice_2.ASE.spp` usa `ice_2.smd`) e
  desenhado de cima (so geometria, sem texturas), colorido por altura, com o norte (Z maior) para cima. Se o `.smd` nao estiver
  la, use "Browse map file..." (o `.smd` do cliente serve) ou edite os pontos sem o fundo.
- O servidor le o `.spp` ao iniciar: reinicie-o depois de salvar.

## Cuidados

- Para editar um personagem ele precisa estar deslogado: o servidor regrava o arquivo ao sair.
- Antes de gravar, o PTEditor cria um backup (`Backup\` nas pastas do GameServer e `PTEditor_Backup\` no DataServer).
- A exclusao de personagem segue a do servidor: libera o slot da conta, guarda uma copia em `DataServer\deleted\`
  (Lv 10+) e apaga o arquivo. Personagem em clan (membro, subchefe ou lider) nao pode ser excluido, como no jogo.
- Os envios do Award Items ficam registrados em `PTEditor\Logs\AwardItems.log` e os do PostBox em `PTEditor\Logs\PostBox.log`.
- PostBox: envie com o jogador deslogado. Se ele ja abriu a caixa na sessao, o logout regrava o arquivo e pode
  desfazer o envio. Itens unicos (Quest) nunca sao oferecidos pelo NPC, por isso a aba nao deixa envia-los.
