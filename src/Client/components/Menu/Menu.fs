module Client.Menu

open Fable.React
open Fable.React.Props
open Fable.Core.JsInterop
open Client.Routes
open Shared
open Elmish.Navigation
open FunctionalComponentView

type Model = {
    User : UserData option
    RenderedOnServer : bool
}

type Props = {
    Model : Model
    OnLogout : unit -> unit
}

let goToUrl (e: Browser.Types.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

let view = elmishView "Menu" (fun (props:Props) ->
    div [ ] [
        yield a [
            Href (toPath Route.Home)
            OnClick goToUrl
        ] [
            str "Home"
        ]
        match props.Model.User with
        | Some _ ->
            yield a [
                OnClick (fun _ -> props.OnLogout())
                OnTouchStart (fun _ -> props.OnLogout())
            ] [
                str "Logout"
            ]
        | _ ->
            yield a [
                Href (toPath Route.Login)
                OnClick goToUrl
            ] [
                str "Login"
            ]
        yield str "Version 1"

        if props.Model.RenderedOnServer then
            yield str " - Rendered on server"
        else
            yield str " - Rendered on client"
    ]
)