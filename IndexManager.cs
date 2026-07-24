namespace capstoneAlgorithms;

public class IndexManager
{
    private readonly AvlTree _tree = new();

    public void LoadFromFile(string path)
    {
        foreach (string line in File.ReadLines(path))
        {
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            int colon = trimmed.IndexOf(':');
            if (colon < 0) continue;

            string term = trimmed[..colon].Trim();
            string pagesPart = trimmed[(colon + 1)..].Trim();

            if (string.IsNullOrEmpty(term)) continue;

            List<int> pages = new();
            foreach (string part in pagesPart.Split(','))
            {
                if (int.TryParse(part.Trim(), out int p))
                    pages.Add(p);
            }

            _tree.Insert(term, pages);
        }
    }

    public void AddManual(string term, List<int> pages)
    {
        _tree.Insert(term, pages);
    }

    public bool DeleteTerm(string term)
    {
        return _tree.Delete(term);
    }

    public void RemovePage(int page)
    {
        _tree.RemovePageFromAll(page);
    }

    public bool RenameTerm(string oldName, string newName)
    {
        List<int>? pages = _tree.Search(oldName);
        if (pages == null) return false;

        _tree.Delete(oldName);
        _tree.Insert(newName, pages);
        return true;
    }

    public List<(string Term, List<int> Pages)> PrefixSearch(string prefix)
    {
        return _tree.StartsWith(prefix);
    }

    public (string Term, int PageCount)? MostFrequent()
    {
        var all = _tree.InOrder();
        if (all.Count == 0) return null;

        (string Term, List<int> Pages) best = all[0];
        for (int i = 1; i < all.Count; i++)
        {
            if (all[i].Pages.Count > best.Pages.Count)
                best = all[i];
        }
        return (best.Term, best.Pages.Count);
    }

    public void Save(string path)
    {
        using StreamWriter writer = new(path);
        foreach (var (term, pages) in _tree.InOrder())
        {
            writer.Write(term);
            writer.Write(": ");
            writer.WriteLine(string.Join(", ", pages));
        }
    }

    public int Load(string path)
    {
        int count = 0;
        foreach (string line in File.ReadLines(path))
        {
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed)) continue;

            int colon = trimmed.IndexOf(':');
            if (colon < 0) continue;

            string term = trimmed[..colon].Trim();
            string pagesPart = trimmed[(colon + 1)..].Trim();

            if (string.IsNullOrEmpty(term)) continue;

            List<int> pages = new();
            foreach (string part in pagesPart.Split(','))
            {
                if (int.TryParse(part.Trim(), out int p))
                    pages.Add(p);
            }

            _tree.Insert(term, pages);
            count++;
        }
        return count;
    }

    public void Display()
    {
        List<(string Term, List<int> Pages)> all = _tree.InOrder();

        if (all.Count == 0)
        {
            Console.WriteLine("  (empty index)");
            return;
        }

        int maxTermLen = 0;
        foreach (var (term, _) in all)
            if (term.Length > maxTermLen) maxTermLen = term.Length;

        foreach (var (term, pages) in all)
        {
            pages.Sort();
            string label = term.PadRight(maxTermLen);
            string pagesStr = string.Join(", ", pages);
            Console.WriteLine($"  {label}  {pagesStr}");
        }
    }
}
