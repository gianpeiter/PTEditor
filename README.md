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
| NPC Editor | Edita NPCs: nome, falas, loja e posicao no mapa (vista de cima do `.smd`, com arrastar e clicar para adicionar). |
| Characters | Lista contas e personagens do `DataServer`, edita level, EXP, atributos, ouro, inventario, rank de classe (libera as skills no Skill Master) e quests, e exclui personagens. |
| Item Finder | Busca itens em todos os personagens e baus, lista itens duplicados e com checksum invalido. |
| Award Items | Envia itens para NPCs com `*AwardItem` (tabela `AwardItem_List` do SQL Server). |
| PostBox | Envia itens para NPCs com `*경품배달`, como o Item Distributor (arquivos `PostBox\<n>\<conta>.dat`). |
| Bellatra | Faz o papel do antigo site de gerencia do Bellatra: gerente e ranking da semana, renda, saudacao da Karina, horario do fechamento e `Bellatra\bellatra.ini` (taxa, monstros e chefes de cada round). |
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

## NPC Editor

- O mapa e a mesma vista de cima do Spawn Point (`Field\*.smd`, achado pelo nome base do `.spc`). Sem o `.smd`, usa o minimapa
  `PTEditor\Maps\*.bmp` como fundo. Roda do mouse da zoom e o botao direito arrasta a vista.
- Cada NPC aparece com a seta de direcao e o nome (caixa **Names**). Clicar num NPC o seleciona na lista; escolher na lista
  centraliza o mapa nele. Arrastar move o NPC: X e Z mudam e o Y acompanha a altura do terreno (o Y gravado nos `.spc`
  e exatamente essa altura). PageUp/PageDown giram o NPC e Delete o apaga.
- Marcando **Click map to add NPCs**, clicar num ponto vazio abre a janela **Add NPC** (o menu Add new NPC da lista abre a
  mesma) e o NPC ja nasce nessa posicao. Desmarcado por padrao para nao abrir a janela sem querer.
- Para trocar um NPC existente, **Ctrl+clique** nele no mapa (nao precisa marcar **Click map to add NPCs**): abre a janela **Replace NPC**
  (a do Add NPC, com o NPC atual selecionado e **Use it as is** como padrao). A troca muda o `.NPC` e o modelo do registro e
  mantem posicao, direcao, ID e Activated; entra no Ctrl+Z e so grava em Save NPC. O `.NPC` e o `.zhoon` do NPC trocado nao
  sao apagados.
- O NPC entra no slot **Empty** que estiver selecionado na lista (selecionar um slot vazio nao mexe na vista do mapa); sem
  um slot vazio selecionado, entra no primeiro livre. O slot aparece no titulo da janela Add NPC.
- A janela **Add NPC** tem a lista de NPCs existentes (nome e arquivo) e o botao **Browse...**, que abre `GameServer\NPC`.
  Duas opcoes: **Create a new NPC from it** (copia o `.NPC` e cria o `.zhoon`; o nome e validado na hora: sem caracteres
  invalidos, caminho de ate 64 caracteres e sem sobrescrever arquivo existente) ou **Use it as is** (so cria o registro no
  mapa, compartilhando o `.NPC` e as falas do original; nenhum arquivo novo).
- Como no Spawn Point, as edicoes (mover, girar, editar os campos ID/X/Y/Z/Angle/Activated/arquivos, adicionar, apagar, Up/Down)
  ficam so em memoria ate **Save NPC**, que grava o `.spc` com backup em `Field\Backup` e o nome/falas no `.zhoon`. O botao
  mostra `*` quando ha alteracoes pendentes, e trocar de mapa ou recarregar com pendencias pede confirmacao.
- Ctrl+Z desfaz e Ctrl+Y (ou Ctrl+Shift+Z) refaz, ate 100 passos por mapa (nos campos de texto vale o desfazer do campo).
  Isso cobre so o `.spc`: loja, nome, falas e o `.NPC`/`.zhoon` criados pelo Add nao sao desfeitos.
- Clique direito sem arrastar mostra **Bring back to Map**, que leva o NPC para o centro da vista.
- O servidor le os `.spc` ao iniciar: reinicie-o depois de salvar.

## Bellatra

No legado, o clan dono do Bellatra configurava o torneio pelo site. O site gravava `Bellatra\bellatra.ini`, que o
servidor rele no inicio de cada SoD; a aba grava o mesmo arquivo, entao a mudanca vale no SoD seguinte, sem reiniciar.

- **Gerente da semana:** como no site, o ranking e semanal. No fechamento (dia e hora em `server.ini` `[Bellatra]`,
  padrao segunda as 15h; campo **Weekly close**) o 1o do ranking vira o gerente ate o proximo fechamento e o ranking
  recomeca. O servidor faz isso sozinho (`SoD2DB.Sod2Config`); a aba mostra o gerente, o periodo e a renda: a taxa do
  dia, entregue ao `ClanMoney` no fechamento diario (mesma hora), o que foi entregue "ontem" e no periodo.
- **Ranking:** os pontos da semana (`ClanDB.CL.Cpoint`), os mesmos 10 que a Karina lista.
- **Saudacao da Karina:** `Sod2Config.Note` do gerente (ate 89 caracteres, sem `|`). E limpa quando outro clan assume.
- **Taxa:** 0 a 10% (como no site), descontada do premio de cada jogador e dada ao gerente. O Save grava o
  `bellatra.ini` e tambem `Sod2Config.TaxRates`, com uma linha em `Sod2TaxLog` quando muda.
- **Rounds 1 a 8:** ate 4 monstros com a % de cada (soma 100), maximo de monstros vivos por sala (11 a 50; o chefe nasce
  depois de maximo - 10) e o chefe dos rounds 3, 6 e 8. Os monstros vem do `*이름` dos `.inf` carregados.
- **Load monsters** preenche os rounds com uma das 3 tabelas do codigo do servidor. **Save** grava as 8 linhas (backup em
  `Bellatra\backup`) e so grava se tudo estiver valido. **Remove file** apaga o arquivo: o servidor volta a sortear uma das
  3 tabelas do codigo a cada SoD.
- Se o arquivo nao tiver as 8 linhas `*BELATRA_MONSTER`, o servidor ignora o arquivo inteiro, inclusive a taxa.

## Cuidados

- Para editar um personagem ele precisa estar deslogado: o servidor regrava o arquivo ao sair.
- Antes de gravar, o PTEditor cria um backup (`Backup\` nas pastas do GameServer e `PTEditor_Backup\` no DataServer).
- A exclusao de personagem segue a do servidor: libera o slot da conta, guarda uma copia em `DataServer\deleted\`
  (Lv 10+) e apaga o arquivo. Personagem em clan (membro, subchefe ou lider) nao pode ser excluido, como no jogo.
- Os envios do Award Items ficam registrados em `PTEditor\Logs\AwardItems.log` e os do PostBox em `PTEditor\Logs\PostBox.log`.
- PostBox: envie com o jogador deslogado. Se ele ja abriu a caixa na sessao, o logout regrava o arquivo e pode
  desfazer o envio. Itens unicos (Quest) nunca sao oferecidos pelo NPC, por isso a aba nao deixa envia-los.
