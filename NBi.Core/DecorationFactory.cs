﻿using System;
using System.Collections.Generic;
using System.Linq;
using NBi.Core.DataManipulation;
using NBi.Core.Etl;
using NBi.Core.WindowsService;
using NBi.Core.Batch;
using NBi.Core.FileManipulation;
using NBi.Core.Process;
using NBi.Core.Connection;
using NBi.Core.FolderManipulation;
using NBi.Core.Query;

namespace NBi.Core
{
    public class DecorationFactory
    {
        public IDecorationCommandImplementation Get(IDecorationCommand command)
        {
            if (command is IGroupCommand)
            {
                return new GroupCommandFactory().Get(command as IGroupCommand);
            }
            
            if (command is IWindowsServiceCommand)
            {
                return new WindowsServiceCommandFactory().Get(command as IWindowsServiceCommand);
            }

            if (command is IDataManipulationCommand)
            {
                return new DataManipulationFactory().Get(command as IDataManipulationCommand);
            }

            if (command is IBatchCommand)
            {
                return new BatchRunnerFactory().Get(command as IBatchCommand);
            }

            if (command is IEtlRunCommand)
            {
                return new EtlRunnerFactory().Get(command as IEtlRunCommand);
            }

            if (command is IFileManipulationCommand)
            {
                return new FileManipulationFactory().Get(command as IFileManipulationCommand);
            }

            if (command is IFolderManipulationCommand)
            {
                return new FolderManipulationFactory().Get(command as IFolderDeleteCommand);
            }

            if (command is IProcessCommand)
            {
                return new ProcessCommandFactory().Get(command as IProcessCommand);
            }

            if (command is IConnectionWaitCommand)
            {
                return new ConnectionWaitFactory().Get(command as IConnectionWaitCommand);
            }


            throw new ArgumentException();
        }

        public IDecorationCheckImplementation Get(IDecorationCheck check)
        {
            if (check is IWindowsServiceCheck)
            {
                return new WindowsServiceCheckFactory().Get(check as IWindowsServiceCheck);
            }

            throw new ArgumentException();
        }

    }
}
