# Book Index Manager

Gestor de índice de términos para libros. Implementa un **AVL Tree** custom para almacenar términos y sus páginas de forma ordenada y eficiente.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Compilar y ejecutar

```bash
cd capstoneAlgorithms
dotnet build
dotnet run
```

## Cómo cargar archivos

El programa lee archivos de texto plano con el siguiente formato:

```
Termino: pagina1, pagina2, pagina3
Otro termino: pagina5
```

Reglas del formato:
- Un término por línea.
- El término y las páginas se separan con `:` (dos puntos).
- Las páginas se separan con `,` (coma).
- Se ignoran líneas vacías.

### Ejemplo: `indice.txt`

```
Arrays: 20, 21, 22
Binary search: 30
Binary search tree: 30, 31
Recursion: 40
Sorting: 45, 46, 47, 50
```

### Cargar desde el menú

```
=== Book Index Manager ===
...
1. Load from file
...
Choose: 1
File path: indice.txt
Loaded.
```

Si el archivo no existe, el programa muestra `File not found.`

## Menú de opciones

| Opción | Acción |
|--------|--------|
| **1. Load from file** | Carga términos desde un archivo de texto. |
| **2. Add term manually** | Ingresa un término y sus páginas manualmente. |
| **3. Delete term** | Elimina un término completo del índice. |
| **4. Remove page from all terms** | Elimina una página de todos los términos que la contengan. |
| **5. Rename term** | Cambia el nombre de un término conservando sus páginas. |
| **6. Prefix search** | Busca todos los términos que comiencen con un prefijo. |
| **7. Most frequent term** | Muestra el término que aparece en la mayor cantidad de páginas. |
| **8. Save index** | Guarda el índice actual en un archivo de texto. |
| **9. Load index from saved file** | Carga un índice previamente guardado. |
| **10. Display index** | Muestra el índice completo con formato alineado. |
| **0. Exit** | Sale del programa. |

## Estructura de datos

**AVL Tree** — árbol binario de búsqueda auto-balanceable.

| Operación | Complejidad (peor caso) |
|-----------|-------------------------|
| Insertar término | O(log n) |
| Buscar término | O(log n) |
| Eliminar término | O(log n) |
| Búsqueda por prefijo | O(n) |
| Término más frecuente | O(n) |
| Mostrar índice | O(n) |

Se eligió AVL Tree sobre Trie y BST porque:
- **vs BST**: garantiza O(log n) en todos los casos (BST degenera a O(n) con datos ordenados).
- **vs Trie**: el Trie es superior en búsqueda por prefijo, pero requiere más memoria y las demás operaciones (renombrar, eliminar páginas) son más complejas de implementar. Para el volumen de datos de un libro, O(n) en prefijo es aceptable.

## Archivos del proyecto

```
AvlTree.cs         → Implementación del AVL Tree
IndexManager.cs    → Lógica de negocio (7 operaciones)
Program.cs         → Menú interactivo por consola
capstoneAlgorithms.csproj
capstoneAlgorithms.slnx
.gitignore
```
