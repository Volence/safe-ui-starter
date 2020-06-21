namespace CustomComponents

open Fable.React
open Fable.React.Props
open Fable.MaterialUI.Core
open Fable.MaterialUI.Props
open Elmish
open Elmish.Navigation


module SideNav =
    let sideBarWidth = "240px"

    let drawerStyle = [
        CSSProp.MinWidth sideBarWidth
    ]

    let changeUrl href _ =
        Navigation.newUrl href |> List.map (fun f -> f ignore) |> ignore


    let sideNav content =
        let listItems = content |> List.map (fun data ->
                match data with
                | ("","") -> div[][]
                | (textItem:string,"") ->
                    listItem [
                        ListItemProp.Button true
                    ] [
                        listItemText [] [
                            typography [
                                TypographyProp.Variant TypographyVariant.Body1
                                MaterialProp.Color ComponentColor.Inherit
                            ] [
                                str textItem
                            ]
                        ]
                    ]
                | (textItem:string,url:string) ->
                    listItem [
                        ListItemProp.Button true
                        OnClick <| (changeUrl url)
                    ] [
                        listItemText [] [
                            typography [
                                TypographyProp.Variant TypographyVariant.Body1
                                MaterialProp.Color ComponentColor.Inherit
                            ] [
                                str textItem
                            ]
                        ]
                    ]
            )

        list [
            Style drawerStyle
        ] listItems

    let main data =
        drawer [
            DrawerProp.Variant DrawerVariant.Permanent
        ] [
            toolbar [] []
            sideNav data
        ]