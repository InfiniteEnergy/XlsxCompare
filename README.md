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

## Usage

You can download a pre-built binaries from the github releases page.

```console
$ ./XlsxCompare config.json left.xlsx right.xlsx
[20:54:44] info: XlsxCompare.Driver[0]
      Starting
[20:54:44] info: XlsxCompare.Driver[0]
      Reading config from config.json
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

A complicated json file controls how the two files will be compared. Here's a small example:

```json
{
  "leftKeyColumn": "Id",
  "rightKeyColumn": "OLD_ID",
  "resultOptions": {
    "leftValueHeader": "my value",
    "rightValueHeader": "your value",
    "leftColumnNames": [
      "Batch #"
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

This config will:

* join the two xlsx files on `Id == OLD_ID`
* checks for `Name == CUSTOMER_NAME`, ignoring case and normalizing nulls and
  whitespace (e.g. `"Ryan     "` will match `"RYAN"` and `null` will match `""`)
* checks for `DateAdded == DT_CREATE`, parsing both into dates before comparing
  (e.g. `"2021-04-02"` will match `"4/2/2021 3:45PM"`)
* the output `results.xlsx` will have a row for each mismatch, with these columns:
  * `Batch #` - value from the left file's `Batch #` column
  * `NEW_ID` - value from the right file's `NEW_ID` column
  * `Mismatched field` - either `Name` or `DateAdded`, depending on which assertion failed
  * `my value` - mismatched value from the left file
  * `your value` - mismatched value from the right file

### Top-level configuration

|key|meaning|default|
|-|-|-|
|`leftKeyColumn`|column name in the "left" file that contains a primary key||
|`rightKeyColumn`|column name in the "right" file that matches the primary key||
|`resultOptions`|object configuring the result file, see below||
|`assertions`|an array of assertion objects configuring how we want columns to match, see below||
|`ignoreMissingRows`|allow rows to exist in the "left" without with a match in the "right" file, useful for checking partial output|`false`|

### `resultOptions` configuration

|key|meaning|default|
|-|-|-|
|`path`|name of the xlsx file to write|`results.xlsx`|
|`leftValueHeader`|header to use over `leftKeyColumn` values|`left value`|
|`rightValueHeader`|header to use over `rightKeyColumn` values|`right value`|
|`leftColumnNames`|additional data to include from the left file. This is useful for adding context to help analyse the mismatches.|`null`|
|`rightColumnNames`|additional data to include from the right file|`null`|

### `assertion` configuration

|key|meaning|default|
|-|-|-|
|`leftColumnName`|column to compare from the left file||
|`rightColumnName`|column to compare from the right file||
|`matchBy`|how to compare the two values, see `matchBy` below|`string`|
|`remove`|if present: before comparison, remove this string from both values|`null`|
|`zeroRepresentsEmpty`|if true: before comparison, convert any zero values (e.g. `0`, `0.0`) to empty string|`false`|

### `matchBy` options

|`matchBy`|rule|examples|
|-|-|-|
|`string`|strings must match, ignoring case and leading/trailing whitespace|`test` matches `test,` `TEST` and ` Test `, but not `testing`|
|`stringIgnoreMissingLeft`|same as `string`, but treat a missing "left" value as a match|same as `string`, but an empty string matches `test`|
|`integer`|parse to integers before comparison|`0123` matches `123`|
|`decimal`|parse to decimals before comparison|`0.123` matches `.123000`|
|`date`|parse as dates before comparison|`2021-04-02` matches `20210402` and `4/2/2021 3:45PM`, but not `2021-04-03`|
|`stringLeftStartsWithRight`|the left value must start with the right value|`testing` matches `test`, but not `testing with suffix`|
|`stringRightStartsWithLeft`|the right value must start with the left value|`test` matches `testing`|

## Developing

* uses `net5`
* recommend using [Visual Studio Code](https://code.visualstudio.com/), or
  another IDE that supports [EditorConfig](https://editorconfig.org/)
* releases are based on git tags: make a tag like `v${SEMVER}` and CI will
  create a github release with standalone binaries
