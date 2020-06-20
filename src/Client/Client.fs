module Client

open Elmish
// open Elmish.React
open Fable.React
open Fable.React.Props
open Fable.MaterialUI.Core
open Fable.MaterialUI.Props
// open Fetch.Types
open Thoth.Fetch
// open Thoth.Json

open CustomComponents

open Shared

let sideBarWidth = "240px"

let rootStyle = [
    CSSProp.Display DisplayOptions.Flex
    CSSProp.FlexWrap "wrap"
]

let mainContainerStyle = [
    CSSProp.Width ("calc(100% - " + sideBarWidth + ")")
    CSSProp.MarginLeft "auto"
]

let mainContentStyle = [
    CSSProp.Padding "2rem"
]

let toolbarWidth = [
    CSSProp.Width "100%"
]
// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = { Counter: Counter option }

// The Msg type defines what events/actions can occur while the application is running
// the state of the application changes *only* in reaction to these events
type Msg =
    | Increment
    | Decrement
    | InitialCountLoaded of Counter

let initialCounter () = Fetch.fetchAs<unit, Counter> "/api/init"

// defines the initial state and initial command (= side-effect) of the application
let init () : Model * Cmd<Msg> =
    let initialModel = { Counter = None }
    let loadCountCmd =
        Cmd.OfPromise.perform initialCounter () InitialCountLoaded
    initialModel, loadCountCmd

// The update function computes the next state of the application based on the current state and the incoming events/messages
// It can also run side-effects (encoded as commands) like calling the server via Http.
// these commands in turn, can dispatch messages to which the update function will react.
let update (msg : Msg) (currentModel : Model) : Model * Cmd<Msg> =
    match currentModel.Counter, msg with
    | Some counter, Increment ->
        let nextModel = { currentModel with Counter = Some { Value = counter.Value + 1 } }
        nextModel, Cmd.none
    | Some counter, Decrement ->
        let nextModel = { currentModel with Counter = Some { Value = counter.Value - 1 } }
        nextModel, Cmd.none
    | _, InitialCountLoaded initialCount->
        let nextModel = { Counter = Some initialCount }
        nextModel, Cmd.none
    | _ -> currentModel, Cmd.none


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

let show = function
    | { Counter = Some counter } -> string counter.Value
    | { Counter = None } -> "Loading..."

let view (model : Model) (dispatch : Msg -> unit) =
    div [
        Style rootStyle
    ] [
        AppBar.main "SAFE Template"
        // Second one used to fix spacing of the first (can cover content due to fixed position)
        toolbar [
            Style toolbarWidth
        ] []
        SideNav.main ["Item1"; "Item2"; "Item3"; "Item4"; "Item5"]
        div [
            Style mainContainerStyle
        ] [
            div [
                Style mainContentStyle
            ] [
                p [] [
                    str "The initial counter is fetched from server"
                ]
                p [] [
                    str "Press buttons to manipulate counter:"
                ]
                button [
                    ButtonProp.Variant ButtonVariant.Contained
                    MaterialProp.Color ComponentColor.Primary
                    OnClick (fun _ -> dispatch Decrement)
                ] [
                    str "-"
                ]
                div [] [
                    str (show model)
                ]
                button [
                    ButtonProp.Variant ButtonVariant.Contained
                    MaterialProp.Color ComponentColor.Primary
                    OnClick (fun _ -> dispatch Increment)
                ] [
                    str "+"
                ]
            ]
        ]
        Footer.main safeComponents
    ]

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif

Program.mkProgram init update view
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactBatched "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
