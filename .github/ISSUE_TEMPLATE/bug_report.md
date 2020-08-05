---
name: Bug report
about: Create a report to help us improve
title: "[Bug]"
labels: bug
assignees: ''

---

**Describe the bug**
A clear and concise description of what the bug is.

**To Reproduce**
Please create a standalone Page which reproduces the issue and paste the code here in a code block.
[More Examples](https://github.com/IvanJosipovic/BlazorTable/tree/master/src/BlazorTable.Sample.Shared/Bugs)

```
@page "/Bug152"

<Table TableItem="PersonData" Items="data" ShowSearchBar="true" ShowFooter="true">
    <Column TableItem="PersonData" Title="Id" Field="@(x => x.ShortId)" Sortable="false" Filterable="true" SetFooterValue="Count" />
    <DetailTemplate TableItem="PersonData">
        @context.ShortId
    </DetailTemplate>
</Table>

@code
{
    private PersonData[] data;

    protected override void OnInitialized()
    {
        data = new PersonData[]
        {
            new PersonData()
            {
                ShortId = 5
            }
        };
    }

    public class PersonData
    {
        public int ShortId { get; set; }
    }
}
```


**Expected behavior**
A clear and concise description of what you expected to happen.
