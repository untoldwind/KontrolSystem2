# std::rendezvous::dock



## Functions


### choose_docking_nodes

```rust
pub sync fn choose_docking_nodes ( vessel : ksp::vessel::Vessel,
                                   target : ksp::vessel::Targetable ) -> Result<(target_port : ksp::vessel::ModuleDockingNode, vessel_port : ksp::vessel::ModuleDockingNode)>
```



Parameters

| Name   | Type                    | Optional | Description |
| ------ | ----------------------- | -------- | ----------- |
| vessel | ksp::vessel::Vessel     |          |             |
| target | ksp::vessel::Targetable |          |             |


### dock_approach

```rust
pub fn dock_approach ( vessel : ksp::vessel::Vessel,
                       target_port : ksp::vessel::ModuleDockingNode ) -> Result<Unit>
```



Parameters

| Name        | Type                           | Optional | Description |
| ----------- | ------------------------------ | -------- | ----------- |
| vessel      | ksp::vessel::Vessel            |          |             |
| target_port | ksp::vessel::ModuleDockingNode |          |             |


### dock_move_correct_side

```rust
pub fn dock_move_correct_side ( vessel : ksp::vessel::Vessel,
                                target_port : ksp::vessel::ModuleDockingNode ) -> Result<Unit>
```



Parameters

| Name        | Type                           | Optional | Description |
| ----------- | ------------------------------ | -------- | ----------- |
| vessel      | ksp::vessel::Vessel            |          |             |
| target_port | ksp::vessel::ModuleDockingNode |          |             |


### dock_vessel

```rust
pub fn dock_vessel ( vessel : ksp::vessel::Vessel,
                     target : ksp::vessel::Targetable ) -> Result<Unit>
```



Parameters

| Name   | Type                    | Optional | Description |
| ------ | ----------------------- | -------- | ----------- |
| vessel | ksp::vessel::Vessel     |          |             |
| target | ksp::vessel::Targetable |          |             |

