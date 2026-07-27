# Book Index Manager

# Intruduccion

¿Alguna vez has visto el índice al final de un libro técnico? Ese que dice "Algorithm: 1, 4, 7-9" o "Data structures: 10, 12-15". Pues esto es exactamente eso, pero en software.

El programa toma un libro de texto, extrae los términos relevantes y construye un índice ordenado automáticamente. Puedes buscar términos, editarlos, guardar el índice y cargarlo después.

Todo está construido sobre un **AVL Tree** implementado desde cero, sin usar colecciones ordenadas de la biblioteca estándar.

---

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Compilar y ejecutar

```bash
cd capstoneAlgorithms
dotnet build
dotnet run
```

---

## El menú paso a paso

Apenas ejecutas el programa, aparece un menú con 10 opciones. La idea es que empieces cargando un archivo (opción 1), luego explores el índice (opción 10), hagas búsquedas (opción 6) y cuando termines guardes tu sesión (opción 8). Acá está la lista completa:

| Opción | Qué hace |
|--------|----------|
| **1. Load from file** | Carga un archivo de texto plano: cada línea es una página, cada palabra es un término. |
| **2. Add term manually** | Ingresa un término y sus páginas a mano. |
| **3. Delete term** | Elimina un término del índice por completo. |
| **4. Remove page from all terms** | Borra un número de página de todos los términos que la contengan. |
| **5. Rename term** | Cambia el nombre de un término sin perder sus páginas. |
| **6. Prefix search** | Busca todos los términos que empiecen con un prefijo (ej: "alg-" → algorithm, analysis). |
| **7. Most frequent term** | Te dice qué término aparece en la mayor cantidad de páginas. |
| **8. Save index** | Guarda el índice en un archivo con formato `Term: p1, p2, ...`. |
| **9. Load index from saved file** | Carga un índice que guardaste antes (opción 8). |
| **10. Display index** | Muestra el índice completo, con páginas colapsadas en rangos (1-3 en vez de 1, 2, 3). |
| **0. Exit** | Sale del programa. |

---

## La estructura de datos: AVL Tree

El corazón del proyecto es un **AVL Tree** implementado desde cero. Pero, ¿por qué AVL?

### ¿Por qué no un BST simple?

Un BST es fácil de implementar, pero tiene una debilidad grande: si los datos llegan ordenados —algo muy probable con términos alfabéticos de un índice— el árbol se convierte en una lista enlazada. Tus búsquedas pasan de O(log n) a O(n). Con un AVL, eso no pasa: el árbol se rebalancea después de cada inserción y eliminación, manteniendo la altura en O(log n) siempre.

### ¿Por qué no un Trie?

El Trie es rapidísimo para búsquedas por prefijo (O(k) donde k es el largo del prefijo), pero consume mucha memoria. Cada nodo guarda un arreglo de hijos (normalmente 26+). Para un libro con cientos o miles de términos, el AVL es mucho más liviano y las demás operaciones (renombrar, eliminar páginas) son más naturales de implementar.

### ¿Cómo funciona internamente?

Cada nodo del árbol guarda cuatro cosas:

- **Key**: el término (string).
- **Pages**: una lista de números de página donde aparece.
- **Left / Right**: punteros a los hijos izquierdo y derecho.
- **Height**: la altura del nodo. Es la clave del balance.

```csharp
private class Node
{
    public string Key;
    public List<int> Pages;
    public Node? Left;
    public Node? Right;
    public int Height;
}
```

El **factor de balance** de un nodo se calcula como la altura de su hijo izquierdo menos la altura del hijo derecho. Si el valor absoluto es mayor a 1, el árbol está desbalanceado y hay que rotar.

```
Balance(n) = Height(n.Left) - Height(n.Right)
```

### Las 4 rotaciones

Cuando el árbol se desbalancea, aplicamos una de cuatro rotaciones para corregirlo:

| Caso | Qué pasa | Rotación |
|------|----------|----------|
| **LL** (left-left) | Insertamos a la izquierda del hijo izquierdo. | Rotar a la derecha. |
| **RR** (right-right) | Insertamos a la derecha del hijo derecho. | Rotar a la izquierda. |
| **LR** (left-right) | Insertamos a la derecha del hijo izquierdo. | Rotar izquierda al hijo, luego derecha al padre. |
| **RL** (right-left) | Insertamos a la izquierda del hijo derecho. | Rotar derecha al hijo, luego izquierda al padre. |

Las rotaciones reacomodan los nodos en tiempo constante O(1), y después de cada una el árbol vuelve a estar balanceado.

### Inserción

Insertar un término es un proceso recursivo:

1. Comparamos el término con la clave del nodo actual.
2. Si es menor, vamos a la izquierda; si es mayor, a la derecha.
3. Si el término ya existe (comparación case-insensitive), solo agregamos las páginas nuevas (sin duplicados).
4. Al volver de la recursión, actualizamos la altura del nodo.
5. Calculamos el balance. Si está desbalanceado, aplicamos la rotación correspondiente.

### Eliminación

Eliminar es más complejo porque hay que mantener el balance después de borrar. Hay tres casos:

- **El nodo es una hoja**: lo eliminamos directamente.
- **Tiene un solo hijo**: lo reemplazamos por ese hijo.
- **Tiene dos hijos**: buscamos el sucesor in-order (el nodo más a la izquierda del subárbol derecho), copiamos su clave y páginas al nodo actual, y luego eliminamos el sucesor.

Después de eliminar, volvemos a calcular alturas y balancear igual que en la inserción.

### Búsqueda por prefijo

La búsqueda por prefijo hace un recorrido **in-order** completo del árbol. Visita todos los nodos en orden alfabético y se queda con aquellos cuya clave empiece con el prefijo indicado.

Es O(n) porque en el peor caso hay que recorrer el árbol entero. No podemos hacer mejor que eso con un AVL, pero para el tamaño de un libro (cientos o miles de términos) la respuesta es instantánea.

---

## Cómo funciona cada operación internamente

| Opción del menú | Lo que pasa por dentro |
|---|---|
| **Load from file** | Lee el archivo línea por línea. Cada línea es una página. Cada palabra se convierte a minúsculas y se inserta en el AVL con el número de línea como página. |
| **Add term manually** | Llama a `Insert` directamente. |
| **Delete term** | Busca el término en el AVL, si existe lo elimina y rebalancea el árbol. |
| **Remove page from all terms** | Recorre todo el árbol (in-order) y en cada nodo quita la página indicada. No desbalancea porque solo modifica listas de páginas, no la estructura del árbol. |
| **Rename term** | Busca el término viejo, obtiene sus páginas, lo elimina del árbol y lo inserta con el nuevo nombre. Las páginas se preservan intactas. |
| **Prefix search** | Recorrido in-order filtrando claves que empiecen con el prefijo. Devuelve lista de (término, páginas). |
| **Most frequent term** | Recorrido in-order completo. Lleva un contador del término con más páginas. Devuelve el que tenga el máximo. |
| **Save index** | Recorrido in-order, escribe cada término y sus páginas en formato `Term: p1, p2, ...`. |
| **Load index from saved file** | Parsea cada línea, extrae el término y las páginas, y las inserta en el AVL. |
| **Display index** | Recorrido in-order. Muestra cada término con sus páginas en columnas alineadas. Las páginas se colapsan en rangos: si un término está en las páginas 1, 2, 3 y 5, muestra "1 - 3, 5". |

---

## Complejidad Big-O

| Operación | Complejidad (peor caso) | ¿Por qué? |
|-----------|------------------------|-----------|
| Insertar término | O(log n) | El AVL mantiene altura ~log n, la inserción recorre una rama y hace rotaciones O(1). |
| Buscar término | O(log n) | Misma razón: la altura balanceada garantiza una sola rama. |
| Eliminar término | O(log n) | Recorre una rama para encontrar y eliminar, luego rebalancea en O(log n). |
| Búsqueda por prefijo | O(n) | Recorre todos los nodos del árbol in-order. |
| Término más frecuente | O(n) | Recorre todos los nodos para comparar. |
| Mostrar índice | O(n) | Recorre todos los nodos para mostrarlos. |

Un detalle importante: O(n) para prefijo o más frecuente suena costoso, pero n es el número de términos en el índice. Para un libro de 1000 términos, son 1000 comparaciones — imperceptible.

---

## Archivos del proyecto

```
AvlTree.cs         → Implementación completa del AVL Tree (insert, search, delete, starts-with, etc.)
IndexManager.cs    → Orquestador: conecta el menú con el árbol (10 operaciones)
Program.cs         → Menú interactivo en consola
test.txt           → Archivo chico de prueba ("miguel rojas rojas")
test_large.txt     → Archivo grande de prueba (59 líneas con términos variados)
capstoneAlgorithms.csproj
capstoneAlgorithms.slnx
.gitignore
```

### test_large.txt

Este archivo tiene 59 líneas con términos como "algorithm", "data", "complexity", "analysis", "machine", "sorting" y muchos más. Está diseñado para probar todas las funcionalidades:

- **Carga masiva**: 59 líneas, cada una con varios términos.
- **Términos repetidos**: "algorithm" aparece en múltiples páginas, ideal para Most Frequent.
- **Prefijos compartidos**: términos que empiezan con "alg-", "data-", "comp-" para Prefix Search.
- **Mayúsculas y minúsculas**: mezcla para probar que las búsquedas son case-insensitive.

Para cargarlo, desde el menú elige la opción 1 y escribe `test_large.txt`.

---

## Consideraciones de diseño

- **Todo es case-insensitive**: cuando buscas "Algorithm", también encuentra "algorithm" y "ALGORITHM". Las comparaciones usan `StringComparison.OrdinalIgnoreCase`.
- **Páginas sin duplicados**: si intentas agregar la misma página dos veces a un término, se ignora.
- **Formato de guardado legible**: el archivo que produce la opción 8 se puede leer y editar a mano. Es simple: `Termino: 1, 2, 3` por línea.
- **Sin dependencias externas**: el AVL Tree, el IndexManager y el menú están escritos completamente desde cero. La única dependencia es .NET 10.
