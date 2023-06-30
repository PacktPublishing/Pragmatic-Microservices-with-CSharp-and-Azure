global using Codebreaker.GameAPIs.Endpoints;
global using Codebreaker.GameAPIs.Extensions;
global using Codebreaker.GameAPIs.Models;
global using Codebreaker.GameAPIs.Services;

global using ColorGame = Codebreaker.GameAPIs.Models.Game<Codebreaker.GameAPIs.Models.ColorField, Codebreaker.GameAPIs.Models.ColorResult>;
global using SimpleGame = Codebreaker.GameAPIs.Models.Game<Codebreaker.GameAPIs.Models.ColorField, Codebreaker.GameAPIs.Models.SimpleColorResult>;
global using ShapeGame = Codebreaker.GameAPIs.Models.Game<Codebreaker.GameAPIs.Models.ShapeAndColorField, Codebreaker.GameAPIs.Models.ShapeAndColorResult>;

global using ColorMove = Codebreaker.GameAPIs.Models.Move<Codebreaker.GameAPIs.Models.ColorField, Codebreaker.GameAPIs.Models.ColorResult>;
global using SimpleMove = Codebreaker.GameAPIs.Models.Move<Codebreaker.GameAPIs.Models.ColorField, Codebreaker.GameAPIs.Models.SimpleColorResult>;
global using ShapeMove = Codebreaker.GameAPIs.Models.Move<Codebreaker.GameAPIs.Models.ShapeAndColorField, Codebreaker.GameAPIs.Models.ShapeAndColorResult>;
