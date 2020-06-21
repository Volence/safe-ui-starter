module Client.App

open Fable.Core.JsInterop
open Fable.Import
open Elmish
open Elmish.React
open Elmish.HMR
open Fable.Core
open Elmish.Navigation
open Shared
open Thoth.Json

open Client.Base
open Client.Routes

let handleNotFound (model: Model) =
    JS.console.error("Error parsing url: " + Browser.Dom.window.location.href)
    ( model, Navigation.modifyUrl (toPath Route.NotFound) )

/// The navigation logic of the application given a Route identity parsed from the .../#info
/// information in the URL.
let urlUpdate (result:Route option) (model:Model) =
    match result with
    | None ->
        handleNotFound model

    | Some Route.NotFound ->
        { model with RouteModel = NotFoundModel }, Cmd.none

    | Some Route.Login ->
        let m, cmd = Login.init model.MenuModel.User
        { model with RouteModel = LoginModel m }, Cmd.map LoginMsg cmd

    | Some Route.Home ->
        let subModel, cmd = Home.init()
        { model with RouteModel = HomeRouteModel subModel }, Cmd.map HomeRouteMsg cmd

let loadUser () : UserData option =
    let userDecoder = Decode.Auto.generateDecoder<UserData>()
    match LocalStorage.load userDecoder "user" with
    | Ok user -> Some user
    | Error _ -> None

let hydrateModel (json:string) (route: Route) =
    // The page was rendered server-side and now react client-side kicks in.
    // If needed, the model could be fixed up here.
    // In this case we just deserialize the model from the json and don't need to to anything special.
    let model: Model = Decode.Auto.unsafeFromString(json)
    match route, model.RouteModel with
    | Route.Home, HomeRouteModel ->
    // | Route.Home, HomeRouteModel subModel when subModel.WishList <> None ->
        Some model
    | Route.Login, LoginModel _ ->
        Some model
    | _ ->
        None

let init route =
    let defaultModel () =
        // no SSR
        let model =
            {
                MenuModel = { User = loadUser(); RenderedOnServer = false }
                RouteModel = HomeRouteModel Home.Model.Empty
            }

        urlUpdate route model

    // was the page rendered server-side?
    let stateJson: string option = !!Browser.Dom.window?__INIT_MODEL__

    match stateJson, route with
    | Some json, Some route ->
        // SSR -> hydrate the model
        match hydrateModel json route with
        | Some model ->
            { model with MenuModel = { model.MenuModel with User = loadUser() } }, Cmd.ofMsg AppHydrated
        | _ ->
            defaultModel()
    | _ ->
        defaultModel()

let update msg model =
    match msg, model.RouteModel with
    | StorageFailure e, _ ->
        printfn "Unable to access local storage: %A" e
        model, Cmd.none

    | HomeRouteMsg msg, HomeRouteModel m ->
        let m, cmd = Home.update msg m

        { model with
            RouteModel = HomeRouteModel m }, Cmd.map HomeRouteMsg cmd

    | HomeRouteMsg _, _ -> model, Cmd.none

    | LoginMsg msg, LoginModel m ->
        match msg with
        | Login.Msg.LoginSuccess newUser ->
            model, Cmd.OfFunc.either (LocalStorage.save "user") newUser (fun _ -> LoggedIn newUser) StorageFailure
        | _ ->
            let m, cmd = Login.update msg m

            { model with
                RouteModel = LoginModel m }, Cmd.map LoginMsg cmd

    | LoginMsg _, _ -> model, Cmd.none

    | AppHydrated, _ ->
        { model with MenuModel = { model.MenuModel with RenderedOnServer = false }}, Cmd.none

    | LoggedIn newUser, _ ->
        let nextRoute = Route.Home
        { model with MenuModel = { model.MenuModel with User = Some newUser }},
        Navigation.newUrl (toPath nextRoute)

    | LoggedOut, _ ->
        let subModel, cmd = Home.init()
        { model with
            MenuModel = { model.MenuModel with User = None }
            RouteModel = HomeRouteModel subModel },
        Cmd.batch [
            Navigation.newUrl (toPath Route.Home)
            Cmd.map HomeRouteMsg cmd
        ]

    | Logout(), _ ->
        model, Cmd.OfFunc.either LocalStorage.delete "user" (fun _ -> LoggedOut) StorageFailure

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
|> Program.toNavigable urlParser urlUpdate
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactBatched "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
