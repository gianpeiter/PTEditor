# Arquitetura e registro de erros

Documento de apoio do PTEditor: como o projeto se organiza e o histórico de erros
encontrados (causa, correção e como evitar). Todo erro novo deve entrar na seção
[Registro de erros](#registro-de-erros).

## Estrutura

| Pasta | Papel |
| --- | --- |
| `PTEditor/Data` | Leitura/gravação dos arquivos do servidor (monstros, itens, mapas `.spm`, NPCs `.spc`/`.NPC`, personagens, banco). Sem UI. |
| `PTEditor/Data/Character` | Formatos binários de personagem, conta, baú e itens (checksum, compressão). |
| `PTEditor/Forms` | Telas. `MainEditor` concentra as abas (Monsters, Items, Maps, NPC Editor, Characters, Options). |
| `PTEditor/State` | `AppState`: caminhos do servidor e listas carregadas em memória. |
| `PTEditor/Helpers` | Utilitários (arquivos, log de erros, diálogos, processos). |
| `PTEditor/Maps` | Minimapas `.bmp` usados na aba NPC Editor quando o mapa não tem `.smd` (cópia dos `.tga` do cliente). |

## Fonte de verdade do jogo

Quando um formato ou cálculo do editor divergir do jogo, a referência é o código do
cliente/servidor (PTClassic), não o comportamento observado em outros editores.

| Assunto | Onde está no código do jogo |
| --- | --- |
| Minimapa (pixel ↔ coordenada) | `src/playsub.cpp` → `DrawFieldMap` |
| Mapa em tela cheia | `src/FullZoomMap.cpp` → `SetFzmCharPos` |
| Retângulo de cada cenário | `bin/Client/image/GuideMap/StageInfo.dat` (28 bytes por campo: `RECT`, largura, altura, `float`) |
| Lista e ordem dos campos | `src/field.cpp` → `InitField` (índice = índice no `StageInfo.dat`) |
| Ponto fixo | `FLOATNS = 8` em `src/smLib3d/smType.h` (coordenada do jogo = valor `>> 8`) |
| Registro do `.spc` (NPC fixo) | `smTRNAS_PLAYERINFO` em `Servidor/Network/smPacket.h` (504 bytes: `size`, `code`, `smCHAR_INFO`, `dwObjectSerial`, `x, y, z`, `ax, ay, az`, `state`); lido por `STG_AREA::LoadCharInfoFixed` em `Servidor/Legacy/SrcServer/OnSever.cpp` |
| Modelo e `.NPC` do registro | `szModelName` (offset 40) é o modelo 3D (`SetLoadPattern`); `szModelName2` (offset 0x68) é o `.NPC` (`smCharDecode`), ambos em `STG_AREA::OpenNpc`. Trocar um NPC (Replace) muda os dois e mantém posição, direção e ID |
| ID do NPC | É o `dwObjectSerial`. `STG_AREA::OpenNpc` o sobrescreve com `GetNewObjectSerial()` ao iniciar o mapa; o valor gravado no `.spc` não é usado (0 funciona) |
| Configuração do Bellatra | `Bellatra\bellatra.ini` lido por `Open_SODConfig` (`Servidor\fileread.cpp`) no início de cada SoD (`rsOpen_SODConfig`). Exige as 8 linhas `*BELATRA_MONSTER` (senão ignora o arquivo todo); nome até 31 bytes (`szName[32]`) e linha até 254 bytes (`fgets(line, 255)`). Sem o arquivo, sorteia `sHCoreEvent1/2/3` de `OnSever.cpp` |
| Gerente do Bellatra | `SoD2DB.Sod2Config.MIDX`, trocado por `Bellatra::Tick` no fechamento semanal (`server.ini` `[Bellatra] CloseDay`/`CloseHour`) para o maior `CL.Cpoint` > 0 (desempate por `IDX`); a Karina mostra `Sod2Config.Note` e a renda (`BeforeMoney`, `TotalMoney`) |
| Posição e direção do NPC | `x, y, z` em ponto fixo (o editor lê os 3 bytes altos, `>> 8`); `ay` é a direção, de 0 a 4096. O `y` gravado é a altura do terreno do `.smd` no ponto (`StageMesh.TryHeightAt`) |

## Registro de erros

Modelo de entrada: **Sintoma**, **Causa**, **Correção**, **Como evitar**.

### 2026-10-16 — Ponto do NPC fora do lugar no mapa (NPC Editor)

- **Sintoma:** o ponto do NPC não ficava onde o NPC está. O erro já existia no Pritator.
- **Causa:**
  1. O editor usava origem e escala por mapa, arredondadas à mão (ex.: `village-2` com
     escala vertical 34 contra 35,9 real e centro horizontal deslocado em ~430 unidades).
  2. O cálculo supunha imagem de 256×256. `pilai1`, `forever-fall-01` e `forever-fall-04`
     são 512×512, então posição, tamanho do ponto e arrasto saíam errados.
- **Correção:** o jogo estica a imagem inteira sobre o `StageMapRect` do cenário:
  `px = (x - left) / (right - left) * largura` e `py = (bottom - z) / (bottom - top) * altura`.
  `MainEditor.vb` agora usa a tabela `MapRects` (extraída do `StageInfo.dat`) e o tamanho
  real da imagem. O arrasto e o "Bring back to map" usam o mesmo retângulo.
- **Correções relacionadas:**
  - `ice_2.ASE.spc` apontava para `ice_2.bmp`, que não existe; passou a usar `ice2.bmp`.
  - `NpcData.ReadCoordinate`: o valor `-8388608` (24 bits) era lido como positivo; o limite
    passou a ser `>=`.
- **Limitações:** `dun-6a` não tem retângulo no jogo e usa o do `fore-3` como fallback;
  `lost_temple` não tem BMP em `Maps` (aparece a imagem de erro).
- **Como evitar:** não calibrar coordenadas "no olho". Derivar do `StageInfo.dat` e, ao
  adicionar um mapa, conferir se todos os NPCs do `.spc` caem dentro do retângulo.

### 2026-10-19 — Mapa some ao selecionar um slot vazio (NPC Editor)

- **Sintoma:** ao clicar em "Empty" na lista de NPCs, o mapa ficava vazio.
- **Causa:** escolher um NPC na lista centraliza a vista nele; o slot vazio tem posição 0, 0,
  fora do mapa.
- **Correção:** só centraliza se o slot está em uso. Slot vazio mostra uma dica na linha de
  status e não mexe na vista. O slot vazio selecionado também é onde o Add NPC cria o NPC.
- **Como evitar:** não usar a posição de um registro sem checar se ele está em uso
  (`NpcData.IsRecordUsed`).

### 2026-10-19 — NPC novo nascia com direção inválida (NPC Editor)

- **Sintoma:** o NPC adicionado mostrava Angle como 805592498 (o valor válido é 0 a 4096) e
  ficava virado para uma direção qualquer.
- **Causa:** o registro-modelo (`NpcData.TemplateHex`) traz lixo no campo de direção.
- **Correção:** `WriteTemplateRecord` zera a direção (norte) ao criar o registro.
- **Como evitar:** ao criar registros a partir de um modelo, definir explicitamente todos os
  campos que o editor exibe.

### 2026-10-19 — Barra de progresso cheia ao abrir o editor

- **Sintoma:** com o programa recém-aberto e nada carregado, a barra de progresso aparecia
  cheia.
- **Causa:** `ShowItemsForDrop` usa a barra do carregamento (`pbWorking`) e a deixava em 100%.
  Com o Item Info habilitado (valor salvo nas configurações) ela roda ao abrir.
- **Correção:** a barra volta a 0 ao terminar a função.
- **Como evitar:** quem mexe em `pbWorking` deve devolvê-la a 0 ao terminar.
