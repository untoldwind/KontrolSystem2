# ksp::telemetry



## Types


### TimeSeries



#### Fields

| Name       | Type   | Read-only | Description                                                                                                                                     |
| ---------- | ------ | --------- | ----------------------------------------------------------------------------------------------------------------------------------------------- |
| end_ut     | float  | R/O       | End time of the time series. This will increase when data is added.                                                                             |
| name       | string | R/O       | Name of the time series. Has to be unique.                                                                                                      |
| resolution | float  | R/O       |  Current time resolution of the time series. This will increase the longer `end_ut - start_ut` gets to prevent accumulation of too much data.   |
| start_ut   | float  | R/O       | Start time of the time series.                                                                                                                  |


#### Methods

##### add_data

```rust
timeseries.add_data ( ut : float,
                      value : float ) -> bool
```

Add a data `value` at time `ut`.


Parameters

| Name  | Type  | Optional | Description |
| ----- | ----- | -------- | ----------- |
| ut    | float |          |             |
| value | float |          |             |


## Functions


### add_time_series

```rust
pub sync fn add_time_series ( name : string,
                              startUt : float,
                              initialResolution : float ) -> ksp::telemetry::TimeSeries
```


Create a new time series starting at `startUt` with an initial resultion `initialResolution`.
If a time series of the `name` already exists it will be replace by the new one.



Parameters

| Name              | Type   | Optional | Description |
| ----------------- | ------ | -------- | ----------- |
| name              | string |          |             |
| startUt           | float  |          |             |
| initialResolution | float  |          |             |


### get_time_series

```rust
pub sync fn get_time_series ( name : string ) -> Option<ksp::telemetry::TimeSeries>
```

Get a time series by name. Will be undefined if there it does not exists


Parameters

| Name | Type   | Optional | Description |
| ---- | ------ | -------- | ----------- |
| name | string |          |             |


### remove_all_time_series

```rust
pub sync fn remove_all_time_series ( ) -> Unit
```

Remove all time series.


### remove_time_series

```rust
pub sync fn remove_time_series ( name : string ) -> bool
```

Remove a time series by name.


Parameters

| Name | Type   | Optional | Description |
| ---- | ------ | -------- | ----------- |
| name | string |          |             |


### save_time_series

```rust
pub sync fn save_time_series ( filename : string ) -> Unit
```

Store the data of all time series to a file.


Parameters

| Name     | Type   | Optional | Description |
| -------- | ------ | -------- | ----------- |
| filename | string |          |             |

