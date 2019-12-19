# BlazorTable
[![Demo](https://img.shields.io/badge/Live-Demo-Blue?style=flat-square)](https://BlazorTable.netlify.com/)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BlazorTable.svg?style=flat-square)](https://www.nuget.org/packages/BlazorTable)
[![Nuget (with prereleases)](https://img.shields.io/nuget/dt/BlazorTable.svg?style=flat-square)](https://www.nuget.org/packages/BlazorTable)
![](https://github.com/IvanJosipovic/BlazorTable/workflows/CI/CD/badge.svg)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=IvanJosipovic_BlazorTable&metric=alert_status)](https://sonarcloud.io/dashboard?id=IvanJosipovic_BlazorTable)

**Work in progress!** 

A simple Table Control for Blazor with Sorting, Paging and Filtering

## Install

- Add Nuget [BlazorTable](https://www.nuget.org/packages/BlazorTable)
- Add ```<script src="_content/BlazorTable/BlazorTable.min.js"></script>``` to the index.html or _Hosts.cshtml

## Features
- Column Reordering
- Edit Mode ([Template Switching](/src/BlazorTable.Sample.Shared/Pages/EditMode.razor))
- Client Side
	- Paging
	- Sorting
    - Filtering
      	- Strings
        - Numbers
        - Dates
        - Enums
        - Custom Component
## Dependencies
- Bootstrap 4 CSS

## Sample
[Example](/src/BlazorTable.Sample/Pages/Index.razor)

```csharp
<Table TableItem="PersonData" Items="data" PageSize="15">
    <Column TableItem="PersonData" Title="Id" Field="@(x => x.id)" Sortable="true" Filterable="true" Width="10%" />
    <Column TableItem="PersonData" Title="First Name" Field="@(x => x.first_name)" Sortable="true" Filterable="true" Width="20%" />
    <Column TableItem="PersonData" Title="Last Name" Field="@(x => x.last_name)" Sortable="true" Filterable="true" Width="20%" />
    <Column TableItem="PersonData" Title="Email" Field="@(x => x.email)" Sortable="true" Filterable="true" Width="20%">
        <Template>
            <a href="mailto:@context.email">@context.email</a>
        </Template>
    </Column>
    <Column TableItem="PersonData" Title="Confirmed" Field="@(x => x.confirmed)" Sortable="true" Filterable="true" Width="10%" />
    <Column TableItem="PersonData" Title="Price" Field="@(x => x.price)" Sortable="true" Filterable="true" Width="10%" Format="C" Align="Align.Right" />
    <Column TableItem="PersonData" Title="Created Date" Field="@(x => x.created_date)" Sortable="true" Width="10%">
        <Template>
            @context.created_date.ToShortDateString()
        </Template>
    </Column>
    <Pager TableItem="PersonData" ShowPageNumber="true" ShowTotalCount="true" />
</Table>
```
