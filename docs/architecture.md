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
| `PTEditor/Maps` | Minimapas `.bmp` usados na aba NPC Editor (cópia dos `.tga` do cliente). |

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
