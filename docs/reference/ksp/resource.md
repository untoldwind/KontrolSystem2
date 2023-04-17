# ksp::resource

Collection of types and functions to get information and manipulate in-game resources.


## Types


### FlowDirection

Resource flow direction

#### Methods

##### to_string

```rust
flowdirection.to_string ( ) -> string
```

String representation of the number

### FlowDirectionConstants



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
FLOW_INBOUND | [ksp::resource::FlowDirection](/reference/ksp/resource.md#flowdirection) | R/O | Inbound resource request (i.e demand resource from other parts)
FLOW_OUTBOUND | [ksp::resource::FlowDirection](/reference/ksp/resource.md#flowdirection) | R/O | Outbound resource request (i.e. provide resource to other parts)

#### Methods

##### from_string

```rust
flowdirectionconstants.from_string ( value : string ) -> Option<ksp::resource::FlowDirection>
```

Parse from string

### ResourceContainer



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
list | [ksp::resource::ResourceData](/reference/ksp/resource.md#resourcedata)[] | R/O | 

#### Methods

##### dump_all

```rust
resourcecontainer.dump_all ( ) -> Unit
```



### ResourceData



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
capacity_units | float | R/O | 
resource | [ksp::resource::ResourceDefinition](/reference/ksp/resource.md#resourcedefinition) | R/O | 
stored_units | float | R/O | 

### ResourceDefinition

Represents an in-game resource.


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
display_abbreviation | string | R/O | 
display_name | string | R/O | 
id | int | R/O | 
mass_per_unit | float | R/O | 
mass_per_volume | float | R/O | 
name | string | R/O | 
volume_per_unit | float | R/O | 

### ResourceTransfer

Represents a resource transfer


#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
entries | [ksp::resource::ResourceTransferEntry](/reference/ksp/resource.md#resourcetransferentry)[] | R/O | 
is_running | bool | R/O | 

#### Methods

##### add_container

```rust
resourcetransfer.add_container ( flowDirection : ksp::resource::FlowDirection,
                                 resourceContainer : ksp::resource::ResourceContainer,
                                 relativeAmount : float ) -> bool
```



##### clear

```rust
resourcetransfer.clear ( ) -> Unit
```



##### start

```rust
resourcetransfer.start ( ) -> bool
```



##### stop

```rust
resourcetransfer.stop ( ) -> bool
```



### ResourceTransferEntry



#### Fields

Name | Type | Read-only | Description
--- | --- | --- | ---
flow_direction | [ksp::resource::FlowDirection](/reference/ksp/resource.md#flowdirection) | R/O | 
resource_container | [ksp::resource::ResourceContainer](/reference/ksp/resource.md#resourcecontainer) | R/O | 

## Constants

Name | Type | Description
--- | --- | ---
FlowDirection | ksp::resource::FlowDirectionConstants | Resource flow direction


## Functions


### create_resource_transfer

```rust
pub sync fn create_resource_transfer ( ) -> ksp::resource::ResourceTransfer
```


