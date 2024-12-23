| Property     | Value          
|:-------------|:-----------------------
| Project Name | ConfigurationEditor
| Author       | Liu Wan Li       
| Create Time  | 2019-05-13   

# Summary
这个项目负责配置参数的编辑和维护工作
#---------------------------------------
关于配置参数的一些基本约定
| Author       | Shenyong       
| Create Time  | 2019-05-14
每一台在运行的设备都有一套完整的参数控制该设备的运行。
该套参数我们称为ConfigSets。
每台设备可以存储多个ConfigSets 实例，但是任何时刻只有一套ConfigSets 实例在使用。
ConfigSets 按以下结构组织：
   ConfigSets[+]--+->Category: string   //设备类别
				  +->GUID:string        //唯一编号
				  +->CreatDate:DateTime	//建立日期
				  +->ModifyDate:DateTime //最后修改日期
				  +->ProductSN：string     //产品编号
				  +->EditGeo[+]             //编辑GEO 文件参数,
				  +->Machine[+]             //机械参数
				  +->.......[+]


# IViewLoader Name
ConfigurationEditor
