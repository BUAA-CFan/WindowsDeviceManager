﻿/***************************************************************************
*                                                                          *
* Copyright 2016 Jamie Anderson                                            *
*                                                                          *
* Licensed under the Apache License, Version 2.0 (the "License");          *
* you may not use this file except in compliance with the License.         *
* You may obtain a copy of the License at                                  *
*                                                                          *
*     http://www.apache.org/licenses/LICENSE-2.0                           *
*                                                                          *
* Unless required by applicable law or agreed to in writing, software      *
* distributed under the License is distributed on an "AS IS" BASIS,        *
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. *
* See the License for the specific language governing permissions and      *
* limitations under the License.                                           *
*                                                                          *
***************************************************************************/

using System;
using WindowsDeviceManager.Api;
using WindowsDeviceManager.ValueConverters;

namespace WindowsDeviceManager
{
    public class DeviceRegistryPropertyValue : PropertyValue<DeviceInfo, DeviceRegistryPropertyKey,
        DeviceRegistryPropertyType>
    {
        #region Default converter registration

        static DeviceRegistryPropertyValue()
        {
            ConverterRegistry.Register(new StringConverter(), DeviceRegistryPropertyType.String,
                DeviceRegistryPropertyType.ExpandString, DeviceRegistryPropertyType.ResourceList);

            ConverterRegistry.Register(new StringListConverter(), DeviceRegistryPropertyType.MultiString);

            ConverterRegistry.Register(new UInt32Converter(), DeviceRegistryPropertyType.DoubleWord,
                DeviceRegistryPropertyType.DoubleWordBigEndian);

            ConverterRegistry.Register(new UInt64Converter(), DeviceRegistryPropertyType.QuadWord);

            ConverterRegistry.Register(new ByteArrayConverter(), DeviceRegistryPropertyType.Binary);
        }

        #endregion

        public DeviceRegistryPropertyValue(DeviceRegistryPropertyKey key)
            : base(key)
        {
        }

        /// <summary>
        /// Reads the value of the registry property from the specified device.
        /// </summary>
        /// <param name="deviceInfo">The device to read the property from.</param>
        /// <param name="propertyType">Receives the data type of the property.</param>
        /// <param name="buffer">Receives the raw data that contain the property value.</param>
        /// <returns>
        /// Returns <c>true</c> if the property was read successfully, or <c>false</c> if the property is not
        /// available for the specified device.
        /// </returns>
        /// <exception cref="DeviceManagerSecurityException">
        /// Thrown if the property value cannot be read due to the user having insufficient access rights.
        /// </exception>
        /// <exception cref="DeviceManagerWindowsException">
        /// Throw if an unexpected error occurs while trying to read the value.
        /// </exception>
        protected override bool ReadValue(DeviceInfo deviceInfo, out DeviceRegistryPropertyType propertyType,
            out Api.Buffer buffer)
        {
            if (!SetupDi.GetDeviceRegistryProperty(deviceInfo, Key, out propertyType, out buffer))
            {
                // Only "not found" errors are valid failures.
                var lastError = ErrorHelpers.GetLastError();
                if (lastError == ErrorCode.NotFound)
                {
                    return false;
                }

                // Everything else is an unexpected failure.
                throw ErrorHelpers.CreateException(lastError, "Unable to query device registry property.");
            }

            return true;
        }
    }
}
