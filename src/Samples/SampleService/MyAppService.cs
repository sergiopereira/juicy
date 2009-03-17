﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Juicy.WindowsService;

[assembly: ServiceRegistration(
	SampleService.MyAppService.MyAppSvcName, // <-- just a constant string
	"MyApp Support Service",
	"Supports the MyApp application performing several critical background tasks.")
]

namespace SampleService
{

    partial class MyAppService : SPServiceBase
    {
        public const string MyAppSvcName = "MyAppSVC";

        public MyAppService()
        {
            InitializeComponent();

            //Important.. use the constant here AFTER the call to InitializeComponent()
            this.ServiceName = MyAppSvcName;
        }
        
    }
}