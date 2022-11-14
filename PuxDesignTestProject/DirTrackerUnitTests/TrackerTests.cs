using DirTrackerBL.Enums;
using DirTrackerBL.Facades;
using DirTrackerBL.Interfaces;
using DirTrackerBL.Services;
using DirTrackerDAL.Interfaces;
using DirTrackerDAL.Models;
using FluentAssertions;
using Moq;

namespace DirTrackerUnitTests;

public class TrackerTests
{
    private Mock<IDalFileManager> _dalFileManagerMock;
    private Mock<IXmlSerializer> _serializerMock;
    private Mock<IHashService> _hasherMock;
    private Mock<IFileService> _blFileServiceMock;

    public TrackerTests()
    {
        _dalFileManagerMock = new Mock<IDalFileManager>();
        _serializerMock = new Mock<IXmlSerializer>();
        _hasherMock = new Mock<IHashService>();
        _blFileServiceMock = new Mock<IFileService>();
    }

    [Fact]
    public async Task GetChanges_InvalidPath_InvalidStatus_Async()
    {
        var invalidPath = "";
        _blFileServiceMock
            .Setup(x => x.DirExists(invalidPath))
            .Returns(false);

        ITrackerFacade tracker = new TrackerFacade(_dalFileManagerMock.Object,
            _serializerMock.Object, _hasherMock.Object, _blFileServiceMock.Object);

        var (changes, status) = await tracker.GetChanges(invalidPath);
        status.Should().Be(InputStatus.InvalidDir);
        changes.Should().BeEmpty();
    }

    [Fact]
    public async Task GetChanges_NewEmptyPath_NewDirStatus_Async()
    {
        var validPath = "C:\\";
        _blFileServiceMock
            .Setup(x => x.DirExists(validPath))
            .Returns(true);
        _blFileServiceMock
            .Setup(x => x.GetDirNamesInDir(It.IsAny<string>()))
            .Returns(new List<string>());
        _blFileServiceMock
            .Setup(x => x.GetFileNamesInDir(It.IsAny<string>()))
            .Returns(new List<string>());

        _dalFileManagerMock
            .Setup(x => x.ReadFileContent())
            .ReturnsAsync("");

        _serializerMock
            .Setup(x => x.Serialize(It.IsAny<DirModel>()))
            .Returns("");


        ITrackerFacade tracker = new TrackerFacade(_dalFileManagerMock.Object,
            _serializerMock.Object, _hasherMock.Object, _blFileServiceMock.Object);

        var (changes, status) = await tracker.GetChanges(validPath);
        status.Should().Be(InputStatus.NewDir);
        changes.Should().HaveCount(1);
    }
}