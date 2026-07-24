namespace capstoneAlgorithms;

public class AvlTree
{
    private Node? _root;

    private class Node
    {
        public string Key;
        public List<int> Pages;
        public Node? Left;
        public Node? Right;
        public int Height;

        public Node(string key, List<int> pages)
        {
            Key = key;
            Pages = pages;
            Height = 1;
        }
    }

    private int Height(Node? n) => n?.Height ?? 0;
    private int Balance(Node? n) => Height(n?.Left) - Height(n?.Right);

    private Node RotateRight(Node y)
    {
        Node x = y.Left!;
        Node? t2 = x.Right;
        x.Right = y;
        y.Left = t2;
        y.Height = 1 + Math.Max(Height(y.Left), Height(y.Right));
        x.Height = 1 + Math.Max(Height(x.Left), Height(x.Right));
        return x;
    }

    private Node RotateLeft(Node x)
    {
        Node y = x.Right!;
        Node? t2 = y.Left;
        y.Left = x;
        x.Right = t2;
        x.Height = 1 + Math.Max(Height(x.Left), Height(x.Right));
        y.Height = 1 + Math.Max(Height(y.Left), Height(y.Right));
        return y;
    }

    public void Insert(string term, List<int> pages)
    {
        _root = Insert(_root, term, pages);
    }

    private Node Insert(Node? n, string term, List<int> pages)
    {
        if (n == null) return new Node(term, pages);

        int c = string.Compare(term, n.Key, StringComparison.OrdinalIgnoreCase);
        if (c < 0)
            n.Left = Insert(n.Left, term, pages);
        else if (c > 0)
            n.Right = Insert(n.Right, term, pages);
        else
        {
            foreach (int p in pages)
                if (!n.Pages.Contains(p))
                    n.Pages.Add(p);
            return n;
        }

        n.Height = 1 + Math.Max(Height(n.Left), Height(n.Right));
        int b = Balance(n);

        if (b > 1 && string.Compare(term, n.Left!.Key, StringComparison.OrdinalIgnoreCase) < 0)
            return RotateRight(n);

        if (b < -1 && string.Compare(term, n.Right!.Key, StringComparison.OrdinalIgnoreCase) > 0)
            return RotateLeft(n);

        if (b > 1 && string.Compare(term, n.Left!.Key, StringComparison.OrdinalIgnoreCase) > 0)
        {
            n.Left = RotateLeft(n.Left);
            return RotateRight(n);
        }

        if (b < -1 && string.Compare(term, n.Right!.Key, StringComparison.OrdinalIgnoreCase) < 0)
        {
            n.Right = RotateRight(n.Right);
            return RotateLeft(n);
        }

        return n;
    }

    public List<int>? Search(string term)
    {
        Node? n = _root;
        while (n != null)
        {
            int c = string.Compare(term, n.Key, StringComparison.OrdinalIgnoreCase);
            if (c < 0) n = n.Left;
            else if (c > 0) n = n.Right;
            else return n.Pages;
        }
        return null;
    }

    public bool Delete(string term)
    {
        if (Search(term) == null) return false;
        _root = Delete(_root, term);
        return true;
    }

    private Node? Delete(Node? n, string term)
    {
        if (n == null) return null;

        int c = string.Compare(term, n.Key, StringComparison.OrdinalIgnoreCase);
        if (c < 0)
            n.Left = Delete(n.Left, term);
        else if (c > 0)
            n.Right = Delete(n.Right, term);
        else
        {
            if (n.Left == null) return n.Right;
            if (n.Right == null) return n.Left;

            Node min = GetMin(n.Right);
            n.Key = min.Key;
            n.Pages = min.Pages;
            n.Right = Delete(n.Right, min.Key);
        }

        n.Height = 1 + Math.Max(Height(n.Left), Height(n.Right));
        int b = Balance(n);

        if (b > 1 && Balance(n.Left) >= 0)
            return RotateRight(n);

        if (b > 1 && Balance(n.Left) < 0)
        {
            n.Left = RotateLeft(n.Left!);
            return RotateRight(n);
        }

        if (b < -1 && Balance(n.Right) <= 0)
            return RotateLeft(n);

        if (b < -1 && Balance(n.Right) > 0)
        {
            n.Right = RotateRight(n.Right!);
            return RotateLeft(n);
        }

        return n;
    }

    private Node GetMin(Node n)
    {
        while (n.Left != null) n = n.Left;
        return n;
    }

    public List<(string Term, List<int> Pages)> StartsWith(string prefix)
    {
        var result = new List<(string, List<int>)>();
        CollectPrefix(_root, prefix, result);
        return result;
    }

    private void CollectPrefix(Node? n, string prefix, List<(string, List<int>)> result)
    {
        if (n == null) return;
        CollectPrefix(n.Left, prefix, result);
        if (n.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            result.Add((n.Key, new List<int>(n.Pages)));
        CollectPrefix(n.Right, prefix, result);
    }

    public List<(string Term, List<int> Pages)> InOrder()
    {
        var result = new List<(string, List<int>)>();
        Traverse(_root, result);
        return result;
    }

    private void Traverse(Node? n, List<(string, List<int>)> result)
    {
        if (n == null) return;
        Traverse(n.Left, result);
        result.Add((n.Key, new List<int>(n.Pages)));
        Traverse(n.Right, result);
    }

    public int Count()
    {
        return Count(_root);
    }

    private int Count(Node? n) => n == null ? 0 : 1 + Count(n.Left) + Count(n.Right);

    public void RemovePageFromAll(int page)
    {
        RemovePage(_root, page);
    }

    private void RemovePage(Node? n, int page)
    {
        if (n == null) return;
        n.Pages.Remove(page);
        RemovePage(n.Left, page);
        RemovePage(n.Right, page);
    }

    public bool AddPage(string term, int page)
    {
        Node? n = Find(_root, term);
        if (n == null) return false;
        if (!n.Pages.Contains(page)) n.Pages.Add(page);
        return true;
    }

    public bool RemovePageFromTerm(string term, int page)
    {
        Node? n = Find(_root, term);
        if (n == null) return false;
        return n.Pages.Remove(page);
    }

    private Node? Find(Node? n, string term)
    {
        while (n != null)
        {
            int c = string.Compare(term, n.Key, StringComparison.OrdinalIgnoreCase);
            if (c < 0) n = n.Left;
            else if (c > 0) n = n.Right;
            else return n;
        }
        return null;
    }
}
