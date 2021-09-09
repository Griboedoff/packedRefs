namespace Refs

type Ref =
    { Name: string
      CommitObjectId: string }

module Refs =
    open System
    open System.IO

    let private prependName name ref =
        { ref with
              Name = sprintf "%s/%s" name ref.Name }

    let rec private readRefsRecursively path : Ref seq =
        Directory.EnumerateFileSystemEntries path
        |> Seq.collect
            (fun entry ->
                let name = Path.GetFileName entry

                if Directory.Exists entry then
                    readRefsRecursively entry
                    |> Seq.map (prependName name)
                else
                    let commitId = File.ReadLines entry |> Seq.head

                    Seq.singleton
                        { Name = name
                          CommitObjectId = commitId })

    let private readPackedRefLine (line: string) : Option<Ref> =
        if line.StartsWith '#' || String.IsNullOrEmpty(line) then
            None
        else
            let splitted = line.Split ' '
            Some
                { Name = splitted.[1]
                  CommitObjectId = splitted.[0] }

    let parsePackedRefsLines (lines: string []) =
        lines
        |> Seq.map readPackedRefLine
        |> Seq.where Option.isSome
        |> Seq.map Option.get

    let readPackedRefs (repositoryPath: string) : Ref seq =
        let packedRefsFilePath =
            Path.Combine(repositoryPath, "packed-refs")
        Console.WriteLine (Directory.GetFiles repositoryPath)
        if File.Exists packedRefsFilePath then
            parsePackedRefsLines (File.ReadAllLines packedRefsFilePath)
        else
            Seq.empty


    let rec readRefs (repositoryPath: string) : Ref seq =
        let refsDirectory = Path.Combine(repositoryPath, "refs")

        let refsFromDirectory =
            readRefsRecursively refsDirectory
            |> Seq.map (prependName "refs")

        let packedRefs = readPackedRefs repositoryPath

        Seq.concat [ refsFromDirectory
                     packedRefs ]
        |> Seq.sortBy (fun ref -> ref.Name)
    

