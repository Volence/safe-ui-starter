namespace CustomComponents

open Fable.React
open Fable.React.Props
open Fable.MaterialUI.Core
open Fable.MaterialUI.Props

module SideNav =
    let sideBarWidth = "240px"

    let drawerStyle = [
        CSSProp.MinWidth sideBarWidth
    ]

    let sideNav content =
        let listItems = content |> List.map (fun textItem ->
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