module Client.Routes

open Elmish.UrlParser

/// The different pages of the application. If you add a new page, then add an entry here.
[<RequireQualifiedAccess>]
type Route =
    | Home
    | NotFound
    | Login

let toPath =
    function
    | Route.Home -> "/"
    | Route.Login -> "/login"
    | Route.NotFound -> "/notfound"

/// The URL is turned into a Result.
let routeParser : Parser<Route -> Route,_> =
    oneOf [
        map Route.Home (s "")
        map Route.Login (s "login")
        map Route.NotFound (s "notfound")
    ]

let urlParser location = parsePath routeParser location