module Client.Login

open Elmish
open Fable.React
open Fable.React.Props
open Shared
open System
open Fable.Core.JsInterop
open Fetch.Types
// open Client.Styles
open Fable.MaterialUI.Core
open Fable.MaterialUI.Props
open FunctionalComponentView
#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

type Model = {
    Login : Login
    Running : bool
    ErrorMsg : string option }

type Msg =
    | LoginSuccess of UserData
    | SetUserName of string
    | SetPassword of string
    | AuthError of exn
    | LogInClicked

let authUser (login:Login) = promise {
    if String.IsNullOrEmpty login.UserName then return! failwithf "You need to fill in a username." else
    if String.IsNullOrEmpty login.Password then return! failwithf "You need to fill in a password." else

    let body = Encode.Auto.toString(0, login)

    let props = [
        Method HttpMethod.POST
        Fetch.requestHeaders [ ContentType "application/json" ]
        Body !^body
    ]

    try
        let! res = Fetch.fetch "/api/users/login/" props
        let! txt = res.text()
        return Decode.Auto.unsafeFromString<UserData> txt
    with _ ->
        return! failwithf "Could not authenticate user."
}


let init (user:UserData option) =
    let userName = user |> Option.map (fun u -> u.UserName) |> Option.defaultValue ""

    { Login = { UserName = userName; Password = ""; PasswordId = Guid.NewGuid() }
      Running = false
      ErrorMsg = None }, Cmd.none

let update (msg:Msg) model : Model*Cmd<Msg> =
    match msg with
    | LoginSuccess _ ->
        model, Cmd.none

    | SetUserName name ->
        { model with Login = { model.Login with UserName = name; Password = ""; PasswordId = Guid.NewGuid() } }, Cmd.none

    | SetPassword pw ->
        { model with Login = { model.Login with Password = pw }}, Cmd.none

    | LogInClicked ->
        { model with Running = true },
            Cmd.OfPromise.either authUser model.Login LoginSuccess AuthError

    | AuthError exn ->
        { model with Running = false; ErrorMsg = Some exn.Message }, Cmd.none

type Props = {
    Model: Model
    Dispatch: Msg -> unit
}

let buttonStyles = [
    CSSProp.MarginTop "1rem"
]

let view = elmishView "Login" <| fun { Model = model; Dispatch = dispatch } ->
    let buttonActive =
        if String.IsNullOrEmpty model.Login.UserName ||
           String.IsNullOrEmpty model.Login.Password ||
           model.Running
        then
            "btn-disabled"
        else
            "btn-primary"

    div [] [
        typography [
            TypographyProp.Variant TypographyVariant.H5
            MaterialProp.Color ComponentColor.Inherit
        ] [
            str "Log in with 'test' / 'test'."
        ]

        // Styles.errorBox model.ErrorMsg
        div [] [
            input [
                Id "username"
                HTMLAttr.Type "text"
                Placeholder "Username"
                DefaultValue model.Login.UserName
                DOMAttr.OnChange (fun ev -> dispatch (SetUserName ev.Value))
                AutoFocus true
            ]
        ]
        div [] [
            input [
                Id "password"
                Key ("password_" + model.Login.PasswordId.ToString())
                HTMLAttr.Type "password"
                Placeholder "Password"
                DefaultValue model.Login.Password
                DOMAttr.OnChange (fun ev -> dispatch (SetPassword ev.Value))
                // onEnter LogInClicked dispatch
            ]
        ]
        div [ ] [
            button [
                OnClick (fun _ -> dispatch LogInClicked)
                ButtonProp.Variant ButtonVariant.Contained
                MaterialProp.Color ComponentColor.Primary
                Style buttonStyles
            ] [
                    str "Log In"
            ]
        ]
    ]
