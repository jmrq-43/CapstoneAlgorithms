using capstoneAlgorithms;

IndexManager index = new();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Book Index Manager ===");
    // que cargue los indices desd un archivo
    Console.WriteLine("1. Load from file");
    Console.WriteLine("2. Add term manually");
    Console.WriteLine("3. Delete term");
    //borrar una pagina
    //upda
    Console.WriteLine("4. Remove page from all terms");
    Console.WriteLine("5. Rename term");
    Console.WriteLine("6. Prefix search");
    Console.WriteLine("7. Most frequent term");
    Console.WriteLine("8. Save index");
    Console.WriteLine("9. Load index from saved file");
    Console.WriteLine("10. Display index");
    // guardar sesion
    // cargar sesion
    Console.WriteLine("0. Exit");
    Console.Write("Choose: ");

    string? choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1":
            Console.Write("File path: ");
            string? loadPath = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(loadPath) && File.Exists(loadPath))
            {
                index.LoadFromFile(loadPath);
                Console.WriteLine("Loaded.");
            }
            else
                Console.WriteLine("File not found.");
            break;

        case "2":
            Console.Write("Term: ");
            string? term = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(term)) break;
            Console.Write("Pages (comma-separated): ");
            string? pagesInput = Console.ReadLine()?.Trim();
            List<int> pages = new();
            if (!string.IsNullOrEmpty(pagesInput))
            {
                foreach (string p in pagesInput.Split(','))
                    if (int.TryParse(p.Trim(), out int n))
                        pages.Add(n);
            }
            index.AddManual(term, pages);
            Console.WriteLine("Added.");
            break;

        case "3":
            Console.Write("Term to delete: ");
            string? delTerm = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(delTerm))
                Console.WriteLine(index.DeleteTerm(delTerm) ? "Deleted." : "Not found.");
            break;

        case "4":
            Console.Write("Page number to remove: ");
            if (int.TryParse(Console.ReadLine()?.Trim(), out int page))
            {
                index.RemovePage(page);
                Console.WriteLine("Removed from all terms.");
            }
            break;

        case "5":
            Console.Write("Old term: ");
            string? oldTerm = Console.ReadLine()?.Trim();
            Console.Write("New term: ");
            string? newTerm = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(oldTerm) && !string.IsNullOrEmpty(newTerm))
                Console.WriteLine(index.RenameTerm(oldTerm, newTerm) ? "Renamed." : "Old term not found.");
            break;

        case "6":
            Console.Write("Prefix: ");
            string? prefix = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(prefix)) break;
            var matches = index.PrefixSearch(prefix);
            if (matches.Count == 0)
                Console.WriteLine("No matches.");
            else
                foreach (var (t, p) in matches)
                    Console.WriteLine($"  {t}: {string.Join(", ", p)}");
            break;

        case "7":
            var most = index.MostFrequent();
            if (most == null)
                Console.WriteLine("Index is empty.");
            else
                Console.WriteLine($"  {most.Value.Term} ({most.Value.PageCount} pages)");
            break;

        case "8":
            Console.Write("Save path: ");
            string? savePath = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(savePath))
            {
                index.Save(savePath);
                Console.WriteLine("Saved.");
            }
            break;

        case "9":
            Console.Write("File to load: ");
            string? reloadPath = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(reloadPath) && File.Exists(reloadPath))
            {
                int c = index.Load(reloadPath);
                Console.WriteLine($"Loaded {c} terms.");
            }
            else
                Console.WriteLine("File not found.");
            break;

        case "10":
            Console.WriteLine();
            index.Display();
            break;

        case "0":
            Console.WriteLine("Bye.");
            return;

        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}
