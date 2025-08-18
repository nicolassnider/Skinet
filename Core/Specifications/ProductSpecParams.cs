﻿namespace Core.Specifications;
public class ProductSpecParams : PagingParams
{


    private List<string> _brands = [];

    public List<string> Brands
    {
        get => _brands; //brands = apple,android
        set { _brands = value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList(); }
    }

    private List<string> _types = [];

    public List<string> Types
    {
        get => _types; //types = smartphone,laptop
        set { _types = value.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries)).ToList(); }
    }

    private string? _search;

    public string? Search
    {
        get => _search ?? "";
        set => _search = value.ToLower();
    }


    public string? Sort { get; set; }

}
