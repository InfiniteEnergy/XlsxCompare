# XlsxCompare

Console application to compare two xlsx files containing the same data with
different schema.

This was built to support data migrations, comparing xlsx exports from source
and destination systems to identify bugs in the process.

The files are referred to as the "left" and "right" files, based on the
arguments passed.

For each row in the left file, we find the matching row in right file, and then
compare the two rows with configurable matching rules.

The comparison results are then written to xlsx for further analysis, one row
for each cell that didn't match.

## Example

```console
$ dotnet run --project src/XlsxCompare -- config.json left.xlsx right.xlsx
[20:54:44] info: XlsxCompare.Driver[0]
      Starting
[20:54:44] info: XlsxCompare.Driver[0]
      Reading config from configt.json
[20:54:44] info: XlsxCompare.XlsxComparer[0]
      Comparing left.xlsx to right.xlsx
[20:54:48] info: XlsxCompare.XlsxComparer[0]
      Comparing [XlsxFacade Sheet1, A1:BX9011] to [XlsxFacade Sheet1, A1:BQ9011]
[20:55:09] info: XlsxCompare.XlsxComparer[0]
      Compared
[20:55:09] info: XlsxCompare.ResultsWriter[0]
      Writing results to results.xlsx
[20:55:10] info: XlsxCompare.ResultsWriter[0]
      Written
[20:55:10] info: XlsxCompare.Driver[0]
      Finished
...
```

## Configuration

The config file controls how the two files will be compared. This gets
deserialized to the `CompareOptions` type, see that code for full details.

See `MatchBy` for the different matching rules supported.

### Sample config

This config will:

* join the two xlsx files on `Id == OLD_ID`
* include `Id` and `NEW_ID` for each row in results file
* checks for `Name == CUSTOMER_NAME`, ignoring case and normalizing nulls and
  whitespace (e.g. `"Ryan     "` will match `"RYAN"` and `null` will match `""`)
* checks for `DateAdded == DT_CREATE`, parsing both into dates before comparing
  (e.g. `"2021-04-02"` will match `"4/2/2021 3:45PM"`)

```json
{
  "leftKeyColumn": "Id",
  "rightKeyColumn": "OLD_ID",
  "resultOptions": {
    "leftValueHeader": "my value",
    "rightValueHeader": "your value",
    "leftColumnNames": [
      "Id"
    ],
    "rightColumnNames": [
      "NEW_ID"
    ]
  },
  "assertions": [
    {
      "leftColumnName": "Name",
      "rightColumnName": "CUSTOMER_NAME"
    },
    {
      "leftColumnName": "DateAdded",
      "rightColumnName": "DT_CREATE",
      "matchBy": "Date"
    }
  ]
}
```
