﻿// Copyright 2020-2021 Andreas Atteneder
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using System;
using UnityEngine;

namespace GLTFast {
    public class ConsoleLogger : ICodeLogger {

        public void Error(LogCode code, params string[] messages) {
            Debug.LogError(LogMessages.GetFullMessage(code,messages));
        }
        
        public void Warning(LogCode code, params string[] messages) {
            Debug.LogWarning(LogMessages.GetFullMessage(code,messages));
        }
        
        public void Info(LogCode code, params string[] messages) {
            Debug.Log(LogMessages.GetFullMessage(code,messages));
        }
    }
}

