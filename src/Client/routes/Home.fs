module Client.Home

open Elmish
open Client.Routes
open Elmish.Navigation
open Fable.Core.JsInterop
// open Elmish.React
open Fable.React
open Fable.React.Props
open Fable.MaterialUI.Core
open Fable.MaterialUI.Props
// open Fetch.Types
open Thoth.Fetch
open FunctionalComponentView
// open Thoth.Json
open Shared

// The model holds data that you want to keep track of while the application is running
// in this case, we are keeping track of a counter
// we mark it as optional, because initially it will not be available from the client
// the initial value will be requested from server
type Model = { Counter: Counter option }
    with
        static member Empty : Model = {
            Counter = None
        }

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

let show = function
    | { Counter = Some counter } -> string counter.Value
    | { Counter = None } -> "Loading..."

type Props = {
    Model: Model
    Dispatch: Msg -> unit
}

let goToUrl (e: Browser.Types.MouseEvent) =
    e.preventDefault()
    let href = !!e.target?href
    Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore

let view = elmishView "Home" <| fun { Model = model ; Dispatch = dispatch } ->
    div [] [
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