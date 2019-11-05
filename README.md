# BlazorTable
[![Demo](https://img.shields.io/badge/Live-Demo-Blue?style=flat-square)](https://BlazorTable.netlify.com/)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BlazorTable.svg?style=flat-square)](https://www.nuget.org/packages/BlazorTable)
[![Nuget (with prereleases)](https://img.shields.io/nuget/dt/BlazorTable.svg?style=flat-square)](https://www.nuget.org/packages/BlazorTable)
![](https://github.com/IvanJosipovic/BlazorTable/workflows/CI/CD/badge.svg)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=IvanJosipovic_BlazorTable&metric=alert_status)](https://sonarcloud.io/dashboard?id=IvanJosipovic_BlazorTable)

**Work in progress!**

A simple Table Control for Blazor with Sorting, Paging and Filtering

## Features
- Edit Mode ([Template Switching](/src/BlazorTable.Sample/Pages/EditMode.razor))
- Client Side
	- Paging
	- Sorting
    - Filtering
      	- Strings
        - Numbers
        - Dates
        - Enums
        - Custom Component
## Todo
- Remove dependency on Bootstrap + BlazorStrap


## Sample
[Example](/src/BlazorTable.Sample/Pages/Index.razor)

```csharp
<Table TableItem="PersonData" Items="data" PageSize="15">
    <Column TableItem="PersonData" Title="Id" Field="@(x => x.id)" Sortable="true" Filterable="true" Width="10%">
        <Template>
            @context.id
        </Template>
    </Column>
    <Column TableItem="PersonData" Title="First Name" Field="@(x => x.first_name)" Sortable="true" Filterable="true" Width="20%">
        <Template>
            @context.first_name
        </Template>
    </Column>
    <Column TableItem="PersonData" Title="Last Name" Field="@(x => x.last_name)" Sortable="true" Filterable="true" Width="20%">
        <Template>
            @context.last_name
        </Template>
    </Column>
    <Column TableItem="PersonData" Title="Email" Field="@(x => x.email)" Sortable="true" Filterable="true" Width="20%">
        <Template>
            <a href="mailto:@context.email">@context.email</a>
        </Template>
    </Column>
    <Column TableItem="PersonData" Title="Confirmed" Field="@(x => x.confirmed)" Sortable="true" Filterable="true" Width="10%">
        <Template>
            @context.confirmed.ToString()
        </Template>
    </Column>
    <Column TableItem="PersonData" Title="Fund" Field="@(x => x.fund)" Sortable="true" Filterable="true" Width="10%">
        <Template>
            $@context.fund
        </Template>
    </Column>
    <Column TableItem="PersonData" Title="Created Date" Field="@(x => x.created_date)" Sortable="true" Width="10%">
        <Template>
            @context.created_date.ToShortDateString()
        </Template>
    </Column>
    <Pager TableItem="PersonData" ShowPageNumber="true" ShowTotalCount="true" />
</Table>
```
