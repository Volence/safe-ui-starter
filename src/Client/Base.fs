module Client.Base

open Shared
open CustomComponents
open Fable.MaterialUI.Core
open Client.Routes
open Fable.React
open Fable.React.Props

type RouteModel =
    | HomeRouteModel of Home.Model
    | LoginModel of Login.Model
    | NotFoundModel

type Model = {
    MenuModel : Menu.Model
    RouteModel : RouteModel
}

/// The composed set of messages that update the state of the application
type Msg =
    | AppHydrated
    | HomeRouteMsg of Home.Msg
    | LoginMsg of Login.Msg
    | LoggedIn of UserData
    | LoggedOut
    | StorageFailure of exn
    | Logout of unit

// VIEW

let rootStyle = [
    CSSProp.Display DisplayOptions.Flex
    CSSProp.FlexWrap "wrap"
]

let toolbarWidth = [
    CSSProp.Width "100%"
]

let sideBarWidth = "240px"

let mainContainerStyle = [
    CSSProp.Width ("calc(100% - " + sideBarWidth + ")")
    CSSProp.MarginLeft "auto"
]

let mainContentStyle = [
    CSSProp.Padding "2rem"
]

let safeComponents =
    let components =
        span [ ] [
            a [
                Href "https://github.com/SAFE-Stack/SAFE-template"
            ] [
                str "SAFE  "
                str Version.template
            ]
            str ", "
            a [
                Href "https://saturnframework.github.io"
                ] [
                str "Saturn"
            ]
            str ", "
            a [
                Href "http://fable.io"
            ] [
                str "Fable"
            ]
            str ", "
            a [
                Href "https://elmish.github.io"
            ] [
                str "Elmish"
            ]
        ]

    span [ ] [
        str "Version "
        strong [ ] [
            str Version.app
        ]
        str " powered by: "
        components
    ]

let view model dispatch =
    div [
        Style rootStyle
    ] [
        AppBar.main "SAFE Template"
        // Second one used to fix spacing of the first (can cover content due to fixed position)
        toolbar [
            Style toolbarWidth
        ] []
        SideNav.main [
            ("Home", (toPath Route.Home));
            ("Item1", "");
            ("Item2", "");
            ("Item3", "");
            ("Item4", "");
            ("Item5", "");
            ("Login", (toPath Route.Login))
        ]

        div [
            Style mainContainerStyle
        ] [
            div [
                Style mainContentStyle
            ] [
                match model.RouteModel with
                | HomeRouteModel model ->
                    yield Home.view { Model = model; Dispatch = (HomeRouteMsg >> dispatch) }
                | NotFoundModel ->
                    yield div [] [ str "The page is not available." ]
                | LoginModel m ->
                    yield Login.view { Model = m; Dispatch = (LoginMsg >> dispatch) }
            ]
        ]
        Footer.main safeComponents
    ]